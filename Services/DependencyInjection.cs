using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using netIPAM.Data;
using netIPAM.Identity;
using netIPAM.Services;

namespace netIPAM;

public enum DbProvider { SqlServer, Sqlite }

public static class DependencyInjection
{
    /// <summary>
    /// Enregistre le DbContext en sélectionnant le provider via la configuration :
    ///   "Database:Provider" = "SqlServer" | "Sqlite"
    ///   "Database:ConnectionString" = ...
    /// L'assemblage de migrations correspondant est ciblé automatiquement.
    /// </summary>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration config)
    {
        var providerName = config["Database:Provider"] ?? "Sqlite";
        var connection = config["Database:ConnectionString"]
                         ?? throw new InvalidOperationException("Database:ConnectionString is required");

        if (!Enum.TryParse<DbProvider>(providerName, ignoreCase: true, out var provider))
            throw new InvalidOperationException($"Unknown Database:Provider '{providerName}'");

        services.AddDbContext<AppDbContext>(opt =>
        {
            switch (provider)
            {
                case DbProvider.SqlServer:
                    opt.UseSqlServer(connection, b =>
                        b.MigrationsAssembly("netIPAM"));
                    break;
                case DbProvider.Sqlite:
                    opt.UseSqlite(connection, b =>
                        b.MigrationsAssembly("netIPAM"));
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
        services.AddScoped<PermissionService>();
        services.AddTransient<ScanService>();

        return services;
    }
}
