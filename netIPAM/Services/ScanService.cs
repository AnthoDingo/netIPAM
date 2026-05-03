using Microsoft.EntityFrameworkCore;
using netIPAM.Data;
using netIPAM.Enums;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace netIPAM.Services;

/// <summary>
/// Options de scan réseau.
/// </summary>
public record ScanOptions(
    int MaxParallel = 32,
    int TimeoutMs = 500,
    int Retries = 1,
    bool ResolveDns = true);

/// <summary>
/// Résultat du scan pour un hôte.
/// </summary>
public record ScanHostResult(
    string IpDecimal,
    string IpPresentation,
    bool IsAlive,
    string? Hostname,
    long RoundTripMs,
    DateTime ScannedAt);

/// <summary>
/// Scan réseau par ping + résolution DNS inverse.
///
/// Global : activé si <c>Setting.ScanMaxThreads > 0</c>.
/// Par subnet : activé si <c>Subnet.PingSubnet == true</c>.
/// Plage DHCP : n'importe quel intervalle décimal [start, end].
///
/// Les résultats sont émis via <see cref="IAsyncEnumerable{T}"/> au fil du scan
/// pour une mise à jour en temps réel de l'UI Blazor.
/// </summary>
public class ScanService
{
    private readonly AppDbContext _db;

    public ScanService(AppDbContext db) => _db = db;

    // ── Génération d'adresses ─────────────────────────────────────

    /// <summary>Génère toutes les adresses hôtes utilisables d'un sous-réseau (FirstUsable → LastUsable).</summary>
    public IReadOnlyList<string> GetSubnetAddresses(string networkDecimal, int mask)
    {
        SubnetCalculator.SubnetInfo info = SubnetCalculator.Describe(networkDecimal, mask);
        return GenerateRange(
            IpConverter.ToDecimal(info.FirstUsable),
            IpConverter.ToDecimal(info.LastUsable));
    }

    /// <summary>Génère toutes les adresses d'une plage décimale inclusive [start, end].</summary>
    public IReadOnlyList<string> GetRangeAddresses(string startDecimal, string endDecimal)
        => GenerateRange(startDecimal, endDecimal);

    private static IReadOnlyList<string> GenerateRange(string startDecimal, string endDecimal)
    {
        if (!BigInteger.TryParse(startDecimal, out var start) ||
            !BigInteger.TryParse(endDecimal, out var end) || end < start)
            return [];

        List<string> list = new((int)Math.Min((long)(end - start + 1), 65536));
        for (BigInteger i = start; i <= end; i++)
            list.Add(i.ToString());
        return list;
    }

    // ── Scan principal ────────────────────────────────────────────

    /// <summary>
    /// Scanne une liste d'adresses (décimales) en parallèle.
    /// Produit les résultats au fur et à mesure (IAsyncEnumerable).
    /// </summary>
    public async IAsyncEnumerable<ScanHostResult> ScanAsync(
        IReadOnlyList<string> addresses,
        ScanOptions? options = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        options ??= new ScanOptions();
        if (addresses.Count == 0) yield break;

        // Canal borné : backpressure naturelle, évite d'exploser la RAM
        Channel<ScanHostResult> channel = Channel.CreateBounded<ScanHostResult>(
            new BoundedChannelOptions(options.MaxParallel * 4) { SingleReader = true });

        Task producer = Task.Run(async () =>
        {
            SemaphoreSlim sem = new(options.MaxParallel);
            IEnumerable<Task> tasks = addresses.Select(async decimalAddr =>
            {
                await sem.WaitAsync(ct).ConfigureAwait(false);
                try
                {
                    ScanHostResult result = await ScanOneHostAsync(decimalAddr, options, ct).ConfigureAwait(false);
                    await channel.Writer.WriteAsync(result, ct).ConfigureAwait(false);
                }
                finally { sem.Release(); }
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
            channel.Writer.Complete();
        }, ct);

        await foreach (ScanHostResult result in channel.Reader.ReadAllAsync(ct))
            yield return result;

        await producer; // propage les exceptions du producteur
    }

    /// <summary>
    /// Scanne un subnet complet et met à jour/crée les enregistrements IpAddress en base.
    /// Retourne le nombre d'hôtes détectés actifs.
    /// </summary>
    public async Task<int> ScanAndPersistAsync(
        int subnetId,
        IReadOnlyList<string> addresses,
        ScanOptions? options = null,
        IProgress<(int total, int scanned, int alive)>? progress = null,
        CancellationToken ct = default)
    {
        options ??= new ScanOptions();
        int total = addresses.Count;
        int scanned = 0;
        int alive = 0;

        await foreach (ScanHostResult result in ScanAsync(addresses, options, ct))
        {
            scanned++;
            if (result.IsAlive)
            {
                alive++;
                await UpsertIpAddressAsync(subnetId, result, ct);
            }
            progress?.Report((total, scanned, alive));
        }

        // Mettre à jour lastScan sur le subnet
        Subnet? subnet = await _db.Subnets.FindAsync(new object?[] { subnetId }, ct);
        if (subnet is not null)
        {
            subnet.LastScan = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return alive;
    }

    // ── Vérifications de disponibilité ───────────────────────────

    /// <summary>Retourne vrai si le scan est activé globalement (ScanMaxThreads > 0).</summary>
    public async Task<bool> IsGloballyEnabledAsync(CancellationToken ct = default)
    {
        Setting? s = await _db.Settings.FirstOrDefaultAsync(ct);
        return s is not null && s.ScanMaxThreads > 0;
    }

    /// <summary>Retourne les options de scan depuis la configuration globale.</summary>
    public async Task<ScanOptions> GetOptionsFromSettingsAsync(CancellationToken ct = default)
    {
        Setting? s = await _db.Settings.FirstOrDefaultAsync(ct);
        if (s is null) return new ScanOptions();
        return new ScanOptions(
            MaxParallel: Math.Clamp(s.ScanMaxThreads, 1, 256),
            TimeoutMs: 500,
            Retries: 1,
            ResolveDns: s.EnableDnsResolving);
    }

    // ── Implémentation bas niveau ─────────────────────────────────

    private static async Task<ScanHostResult> ScanOneHostAsync(
        string decimalAddr, ScanOptions options, CancellationToken ct)
    {
        IpVersion version = IpConverter.GuessVersion(decimalAddr);
        string presentation;
        try { presentation = IpConverter.ToPresentation(decimalAddr, version); }
        catch { return new ScanHostResult(decimalAddr, decimalAddr, false, null, 0, DateTime.UtcNow); }

        bool alive = false;
        long rtt = 0;
        string? hostname = null;

        // ── Ping ──
        for (int attempt = 0; attempt <= options.Retries && !alive; attempt++)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                using Ping ping = new();
                PingReply reply = await ping.SendPingAsync(presentation, options.TimeoutMs).ConfigureAwait(false);
                if (reply.Status == IPStatus.Success)
                {
                    alive = true;
                    rtt = reply.RoundtripTime;
                }
            }
            catch { /* hôte inaccessible ou ICMP bloqué */ }
        }

        // ── DNS inverse ──
        if (alive && options.ResolveDns)
        {
            try
            {
                IPHostEntry entry = await Dns.GetHostEntryAsync(presentation, ct).ConfigureAwait(false);
                hostname = entry.HostName;
                // Supprimer le FQDN complet si c'est juste l'IP qui revient
                if (hostname == presentation) hostname = null;
            }
            catch { /* pas de PTR record ou DNS indisponible */ }
        }

        return new ScanHostResult(decimalAddr, presentation, alive, hostname, rtt, DateTime.UtcNow);
    }

    private async Task UpsertIpAddressAsync(int subnetId, ScanHostResult result, CancellationToken ct)
    {
        IpAddress? existing = await _db.IpAddresses
            .FirstOrDefaultAsync(i => i.SubnetId == subnetId && i.IpAddr == result.IpDecimal, ct);

        if (existing is null)
        {
            _db.IpAddresses.Add(new IpAddress
            {
                SubnetId = subnetId,
                IpAddr = result.IpDecimal,
                Hostname = result.Hostname,
                State = 2, // Used
                LastSeen = result.ScannedAt,
                EditDate = result.ScannedAt,
            });
        }
        else
        {
            existing.LastSeen = result.ScannedAt;
            if (!string.IsNullOrEmpty(result.Hostname))
                existing.Hostname = result.Hostname;
        }

        try { await _db.SaveChangesAsync(ct); }
        catch { /* contrainte unique : IP déjà insérée par un thread concurrent */ }
    }
}
