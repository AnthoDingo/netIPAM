using Fido2NetLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhpIpamNet.Infrastructure.Data;
using PhpIpamNet.Infrastructure.Identity;
using PhpIpamNet.Infrastructure.Services;

namespace PhpIpamNet.Infrastructure;

public enum DbProvider { SqlServer, Sqlite }

public static class DependencyInjection
{
    /// <summary>
    /// Enregistre le DbContext en sélectionnant le provider via la configuration :
    ///   "Database:Provider" = "SqlServer" | "Sqlite"
    ///   "Database:ConnectionString" = ...
    /// L'assemblage de migrations correspondant est ciblé automatiquement.
    /// </summary>
    public static IServiceCollection AddPhpIpamPersistence(
        this IServiceCollection services,
        IConfiguration config)
    {
        var providerName = config["Database:Provider"] ?? "Sqlite";
        var connection = config["Database:ConnectionString"]
                         ?? throw new InvalidOperationException("Database:ConnectionString is required");

        if (!Enum.TryParse<DbProvider>(providerName, ignoreCase: true, out var provider))
            throw new InvalidOperationException($"Unknown Database:Provider '{providerName}'");

        services.AddDbContext<PhpIpamDbContext>(opt =>
        {
            switch (provider)
            {
                case DbProvider.SqlServer:
                    opt.UseSqlServer(connection, b =>
                        b.MigrationsAssembly("PhpIpamNet.Migrations.SqlServer"));
                    break;
                case DbProvider.Sqlite:
                    opt.UseSqlite(connection, b =>
                        b.MigrationsAssembly("PhpIpamNet.Migrations.Sqlite"));
                    break;
            }
        });

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<SectionService>();
        services.AddScoped<SubnetService>();
        services.AddScoped<IpAddressService>();
        services.AddScoped<UserService>();
        services.AddScoped<UserGroupService>();
        services.AddScoped<AuthMethodService>();
        services.AddScoped<SettingService>();
        services.AddScoped<LogService>();
        services.AddScoped<VlanService>();
        services.AddScoped<VlanDomainService>();
        services.AddScoped<VrfService>();
        services.AddScoped<DeviceService>();
        services.AddScoped<DeviceTypeService>();
        services.AddScoped<NameserverService>();
        services.AddScoped<CustomerService>();
        services.AddScoped<LocationService>();
        services.AddScoped<IpTagService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<PasskeyService>();

        // Fido2NetLib — configuration via "Fido2" section dans appsettings
        var fido2Section = config.GetSection("Fido2");
        var origins = (fido2Section["Origins"] ?? "https://localhost:7180;http://localhost:5180")
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();

        services.AddFido2(opt =>
        {
            opt.ServerDomain           = fido2Section["Domain"] ?? "localhost";
            opt.ServerName             = fido2Section["ServerName"] ?? "PhpIpamNet";
            opt.Origins                = origins;
            opt.TimestampDriftTolerance = 300_000; // 5 min en ms
        });

        // IMemoryCache requis par PasskeyService pour les challenges temporaires
        services.AddMemoryCache();

        return services;
    }
}
