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
    /// Enregistre le DbContext via une lambda évaluée à chaque création de contexte.
    /// Cela permet à IConfiguration de recharger appsettings.json après le setup
    /// sans redémarrer l'application.
    /// En mode setup (ConnectionString absente), utilise InMemory pour éviter un crash.
    /// </summary>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            // Lecture live — supporte le rechargement de IConfiguration après setup
            IConfiguration cfg        = sp.GetRequiredService<IConfiguration>();
            string? provider   = cfg["Database:Provider"] ?? "Sqlite";
            string? connection = cfg["Database:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connection))
            {
                // Setup non terminé — utiliser InMemory pour que DI ne plante pas
                opt.UseInMemoryDatabase("setup_placeholder");
                return;
            }

            if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
                opt.UseSqlServer(connection, b => b.MigrationsAssembly("netIPAM"));
            else
                opt.UseSqlite(connection, b => b.MigrationsAssembly("netIPAM"));
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
