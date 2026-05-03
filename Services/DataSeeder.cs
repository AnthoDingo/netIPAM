using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using netIPAM.Entities;
using netIPAM.Data;
using netIPAM.Identity;

namespace netIPAM.Services;

/// <summary>
/// Initialise les enregistrements minimaux nécessaires : utilisateur Admin, section
/// par défaut, ligne settings, méthodes d'authentification.
/// À appeler depuis Program.cs après application des migrations :
///   await app.Services.SeedAsync();
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(this IServiceProvider sp, CancellationToken ct = default)
    {
        await using var scope = sp.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");

        // Ligne unique de settings
        if (!await db.Settings.AnyAsync(ct))
        {
            db.Settings.Add(new Setting
            {
                SiteTitle = "phpipam IP address management",
                SiteAdminName = "Sysadmin",
                SiteAdminMail = "admin@domain.local",
                SiteDomain = "domain.local",
                SiteUrl = "http://localhost",
                Version = "1.4",
                SubnetOrdering = "subnet,asc",
                Theme = "dark",
            });
        }

        // Méthodes d'authentification
        if (!await db.UserAuthMethods.AnyAsync(ct))
        {
            db.UserAuthMethods.Add(new UserAuthMethod { Type = "local", Description = "Local database", Protected = "Yes" });
            db.UserAuthMethods.Add(new UserAuthMethod { Type = "http",  Description = "HTTP authentication", Protected = "Yes" });
        }

        // Compte Admin par défaut (mot de passe : ipamadmin)
        if (!await db.Users.AnyAsync(u => u.Username == "Admin", ct))
        {
            var admin = new User
            {
                Username = "Admin",
                RealName = "phpIPAM Admin",
                Email = "admin@domain.local",
                Role = "Administrator",
                AuthMethod = 1,
                Password = hasher.Hash("ipamadmin"),
                PassChange = "Yes",
                Widgets = "statistics;favourite_subnets;changelog;access_logs;error_logs;top10_hosts_v4",
                EditDate = DateTime.UtcNow,
            };
            db.Users.Add(admin);
            logger.LogInformation("Default Admin user created (password: ipamadmin)");
        }

        // Sections de démonstration
        if (!await db.Sections.AnyAsync(ct))
        {
            db.Sections.AddRange(
                new Section { Name = "Customers", Description = "Section for customers" },
                new Section { Name = "IPv6", Description = "Section for IPv6 addresses" });
        }

        // États par défaut (ipTags)
        if (!await db.IpTags.AnyAsync(ct))
        {
            db.IpTags.AddRange(
                new IpTag { Type = "Offline",  ShowTag = 1, BgColor = "#f59c99", FgColor = "#fff", Locked = "Yes", UpdateTag = true },
                new IpTag { Type = "Used",     ShowTag = 0, BgColor = "#a9c9a4", FgColor = "#fff", Locked = "Yes", UpdateTag = true },
                new IpTag { Type = "Reserved", ShowTag = 1, BgColor = "#9ac0cd", FgColor = "#fff", Locked = "Yes", UpdateTag = true },
                new IpTag { Type = "DHCP",     ShowTag = 1, BgColor = "#c9c9c9", FgColor = "#fff", Locked = "Yes", Compress = "Yes", UpdateTag = true });
        }

        await db.SaveChangesAsync(ct);
    }
}
