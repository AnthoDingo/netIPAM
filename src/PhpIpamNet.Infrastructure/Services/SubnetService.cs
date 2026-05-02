using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Domain.Services;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class SubnetService
{
    private readonly PhpIpamDbContext _db;
    public SubnetService(PhpIpamDbContext db) => _db = db;

    public Task<List<Subnet>> ForSectionAsync(int sectionId, CancellationToken ct = default)
        => _db.Subnets
              .Where(s => s.SectionId == sectionId)
              .OrderBy(s => s.IsFolder ? 0 : 1)
              .ThenBy(s => s.SubnetAddress)
              .ToListAsync(ct);

    /// <summary>
    /// Retourne tous les subnets d'une section en ordre hiérarchique (pour le sidebar tree).
    /// Inclut le VLAN pour l'affichage.
    /// </summary>
    public Task<List<Subnet>> ForSectionWithVlanAsync(int sectionId, CancellationToken ct = default)
        => _db.Subnets
              .Where(s => s.SectionId == sectionId)
              .Include(s => s.Vlan)
              .OrderBy(s => s.SubnetAddress)
              .ToListAsync(ct);

    public Task<Subnet?> GetAsync(int id, CancellationToken ct = default)
        => _db.Subnets
              .Include(s => s.Vlan)
              .Include(s => s.Section)
              .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<int> CountIpsAsync(int subnetId, CancellationToken ct = default)
        => await _db.IpAddresses.CountAsync(i => i.SubnetId == subnetId, ct);

    public async Task<Subnet> CreateAsync(Subnet subnet, CancellationToken ct = default)
    {
        subnet.EditDate = DateTime.UtcNow;
        _db.Subnets.Add(subnet);
        await _db.SaveChangesAsync(ct);
        return subnet;
    }

    public async Task UpdateAsync(Subnet subnet, CancellationToken ct = default)
    {
        subnet.EditDate = DateTime.UtcNow;
        _db.Subnets.Update(subnet);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var s = await _db.Subnets.FindAsync(new object?[] { id }, ct);
        if (s is null) return;
        _db.Subnets.Remove(s);
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Statistiques d'occupation d'un sous-réseau (% utilisé).</summary>
    public async Task<(long total, long used, double percentUsed)> UsageAsync(int subnetId, CancellationToken ct = default)
    {
        var subnet = await GetAsync(subnetId, ct);
        if (subnet is null || string.IsNullOrEmpty(subnet.SubnetAddress) || string.IsNullOrEmpty(subnet.Mask))
            return (0, 0, 0d);

        if (!int.TryParse(subnet.Mask, out var maskBits)) return (0, 0, 0d);
        var info = SubnetCalculator.Describe(subnet.SubnetAddress, maskBits);
        var total = (long)info.UsableHosts;
        var used = await CountIpsAsync(subnetId, ct);
        var pct = total == 0 ? 0d : 100.0 * used / total;
        return (total, used, pct);
    }
}
