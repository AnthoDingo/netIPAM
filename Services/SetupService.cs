using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using netIPAM.Data;
using netIPAM.Entities;
using netIPAM.Identity;

namespace netIPAM.Services;

public record TestConnectionResult(bool Success, string? Error, string? Version);

/// <summary>
/// Logique du wizard d'installation :
/// 1. Tester la connexion
/// 2. Appliquer les migrations EF Core
/// 3. Créer le compte administrateur initial
/// 4. Écrire appsettings.json
/// </summary>
public class SetupService
{
    private readonly IWebHostEnvironment _env;

    public SetupService(IWebHostEnvironment env)
    {
        _env = env;
    }

    // ── Construction d'un DbContext ad-hoc ───────────────────────

    public AppDbContext BuildContext(string provider, string connectionString)
    {
        DbContextOptionsBuilder<AppDbContext> opt = new();
        if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
            opt.UseSqlServer(connectionString, b => b.MigrationsAssembly("netIPAM"));
        else
            opt.UseSqlite(connectionString, b => b.MigrationsAssembly("netIPAM"));

        return new AppDbContext(opt.Options);
    }

    // ── Test de connexion ─────────────────────────────────────────

    public async Task<TestConnectionResult> TestConnectionAsync(string provider, string connectionString)
    {
        try
        {
            await using AppDbContext db = BuildContext(provider, connectionString);
            bool canConnect = await db.Database.CanConnectAsync();
            if (!canConnect)
                return new TestConnectionResult(false, "Impossible de se connecter. Vérifiez la chaîne de connexion.", null);

            // Récupérer la version du serveur pour affichage
            string version = db.Database.ProviderName ?? provider;
            return new TestConnectionResult(true, null, version);
        }
        catch (Exception ex)
        {
            return new TestConnectionResult(false, ex.Message, null);
        }
    }

    // ── Application des migrations ────────────────────────────────

    /// <summary>
    /// Applique les migrations EF Core et produit un log ligne par ligne.
    /// Variante ad-hoc (utilisée par le wizard de setup).
    /// </summary>
    public async IAsyncEnumerable<string> ApplyMigrationsAsync(
        string provider, string connectionString)
    {
        yield return "Connexion à la base de données…";

        await using AppDbContext db = BuildContext(provider, connectionString);

        IEnumerable<string> pending;
        string? error = null;
        try
        {
            pending = await db.Database.GetPendingMigrationsAsync();
        }
        catch (Exception ex)
        {
            error = $"❌ Erreur : {ex.Message}";
            pending = [];
        }

        if (error != null)
        {
            yield return error;
            yield break;
        }

        List<string> migrations = pending.ToList();
        if (migrations.Count == 0)
        {
            yield return "✓ Aucune migration en attente — base déjà à jour.";
            yield break;
        }

        yield return $"{migrations.Count} migration(s) à appliquer :";
        foreach (string m in migrations)
            yield return $"  → {m}";

        yield return "Application des migrations…";

        (bool success, string? migError) = await TryApplyMigrationsAsync(db);
        if (success)
            yield return "✓ Migrations appliquées avec succès.";
        else if (migError != null)
            yield return $"❌ Erreur lors des migrations : {migError}";
    }

    private async Task<(bool, string?)> TryApplyMigrationsAsync(AppDbContext db)
    {
        try
        {
            await db.Database.MigrateAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Applique les migrations via un DbContext DI existant (utilisée par la page /migrate).
    /// </summary>
    public async IAsyncEnumerable<string> ApplyFromContextAsync(
        netIPAM.Data.AppDbContext db)
    {
        IEnumerable<string> migrations = await db.Database.GetPendingMigrationsAsync();
        List<string> list = migrations.ToList();

        if (list.Count == 0) { yield return "✓ Base de données déjà à jour."; yield break; }

        yield return $"{list.Count} migration(s) à appliquer :";
        foreach (string m in list) yield return $"  → {m}";
        yield return "";
        yield return "Application en cours…";

        (bool success, string? error) = await TryApplyMigrationsFromContextAsync(db);
        if (success)
            yield return "✓ Toutes les migrations ont été appliquées.";
        else if (error != null)
            yield return $"❌ {error}";
    }

    private async Task<(bool, string?)> TryApplyMigrationsFromContextAsync(AppDbContext db)
    {
        try
        {
            await db.Database.MigrateAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    // ── Création du compte admin ──────────────────────────────────

    public async Task<string?> CreateAdminAsync(
        string provider, string connectionString,
        string username, string email, string password)
    {
        try
        {
            await using AppDbContext db = BuildContext(provider, connectionString);
            BCryptPasswordHasher hasher = new();

            if (await db.Users.AnyAsync(u => u.Username == username))
                return $"L'utilisateur « {username} » existe déjà.";

            // Ligne settings requise (singleton)
            if (!await db.Settings.AnyAsync())
            {
                db.Settings.Add(new Setting
                {
                    SiteTitle     = "netIPAM",
                    SiteAdminName = username,
                    SiteAdminMail = email,
                    Theme         = "dark",
                });
            }

            // Auth method local
            if (!await db.UserAuthMethods.AnyAsync())
            {
                db.UserAuthMethods.Add(new UserAuthMethod
                {
                    Type        = "local",
                    Description = "Local database",
                    Protected   = "Yes",
                });
            }

            // Tags IP par défaut
            if (!await db.IpTags.AnyAsync())
            {
                db.IpTags.AddRange(
                    new IpTag { Type = "Offline",  ShowTag = 1, BgColor = "#f59c99", FgColor = "#fff", Locked = "Yes", UpdateTag = true },
                    new IpTag { Type = "Used",     ShowTag = 0, BgColor = "#a9c9a4", FgColor = "#fff", Locked = "Yes", UpdateTag = true },
                    new IpTag { Type = "Reserved", ShowTag = 1, BgColor = "#9ac0cd", FgColor = "#fff", Locked = "Yes", UpdateTag = true },
                    new IpTag { Type = "DHCP",     ShowTag = 1, BgColor = "#c9c9c9", FgColor = "#fff", Locked = "Yes", Compress = "Yes", UpdateTag = true }
                );
            }

            db.Users.Add(new User
            {
                Username   = username,
                Email      = email,
                Password   = hasher.Hash(password),
                Role       = "Administrator",
                RealName   = "Administrator",
                AuthMethod = 1,
                Disabled   = "No",
                PassChange = "No",
                EditDate   = DateTime.UtcNow,
            });

            await db.SaveChangesAsync();
            return null; // succès
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    // ── Écriture appsettings.json ─────────────────────────────────

    /// <summary>
    /// Met à jour appsettings.json en préservant toutes les clés existantes.
    /// Utilise JsonNode (mutabilité) pour éviter une dépendance Newtonsoft.
    /// </summary>
    public void WriteAppSettings(string provider, string connectionString)
    {
        string path = Path.Combine(_env.ContentRootPath, "appsettings.json");

        JsonNode root;
        if (File.Exists(path))
        {
            string raw = File.ReadAllText(path);
            root = JsonNode.Parse(raw) ?? new JsonObject();
        }
        else
        {
            root = new JsonObject();
        }

        // Section Database
        if (root["Database"] is not JsonObject dbSection)
        {
            dbSection = new JsonObject();
            root["Database"] = dbSection;
        }
        dbSection["Provider"]         = provider;
        dbSection["ConnectionString"] = connectionString;

        // Marquer le setup comme terminé
        if (root["Setup"] is not JsonObject setupSection)
        {
            setupSection = new JsonObject();
            root["Setup"] = setupSection;
        }
        setupSection["Completed"] = true;

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, root.ToJsonString(options));
    }

    // ── Détection si setup requis ─────────────────────────────────

    public bool IsSetupRequired(IConfiguration config)
    {
        string? conn = config["Database:ConnectionString"];
        string? done = config["Setup:Completed"];
        return string.IsNullOrWhiteSpace(conn)
            || !string.Equals(done, "true", StringComparison.OrdinalIgnoreCase);
    }
}
