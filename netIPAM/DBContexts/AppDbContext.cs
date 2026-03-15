using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using netIPAM.Models;

namespace netIPAM.DBContexts
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        private readonly IConfiguration _configuration;
        private readonly string _provider;
        private readonly string _connectionString;

        public DbSet<Setting> Settings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
            _provider = _configuration.GetValue<string>("DatabaseProvider") ?? throw new InvalidOperationException("Database provider not specified in configuration.");

            _connectionString = _configuration.GetConnectionString(_provider) ?? throw new InvalidOperationException($"Connection string for provider '{_provider}' not found.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch(_provider.ToLower())
            {
                case "sqlserver":
                    optionsBuilder.UseSqlServer(_connectionString);
                    break;
                case "sqlite":
                    optionsBuilder.UseSqlite(_connectionString);
                    break;
                //case "postgresql":
                //    optionsBuilder.UseNpgsql(_connectionString);
                //    break;
                default:
                    throw new InvalidOperationException($"Unsupported database provider: {_provider}");
            }

#if DEBUG
            optionsBuilder
                .EnableSensitiveDataLogging();
                //.LogTo(Console.WriteLine, LogLevel.Debug);
#endif
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity => { entity.ToTable("Users"); });
            builder.Entity<IdentityRole>(entity => { entity.ToTable("Roles"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });

            builder.Entity<Setting>().HasData(
                new Setting { Id = 1, Name = "siteTitle", Value = "netIPAM address management", Type = SettingType.String },
                new Setting { Id = 2, Name = "siteDomain", Value = "domain.local", Type = SettingType.String },
                new Setting { Id = 3, Name = "siteURL", Value = "http://yourpublicurl.com", Type = SettingType.String },
                new Setting { Id = 4, Name = "siteLoginText", Value = string.Empty, Type = SettingType.String },
                new Setting { Id = 5, Name = "permissionPropagate", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 6, Name = "vlanMax", Value = 4096.ToString(), Type = SettingType.Integer },
                new Setting { Id = 7, Name = "maintaneanceMode", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 8, Name = "siteAdminName", Value = "Sysadmin", Type = SettingType.String },
                new Setting { Id = 9, Name = "siteAdminMail", Value = "admin@domain.local", Type = SettingType.String },
                new Setting { Id = 10, Name = "api", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 11, Name = "enableIPrequests", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 12, Name = "enableVRF", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 13, Name = "enableNAT", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 14, Name = "enablePowerDNS", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 15, Name = "enableDHCP", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 16, Name = "enableFirewallZones", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 17, Name = "enableDNSresolving", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 18, Name = "tempShare", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 19, Name = "enableChangelog", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 20, Name = "enableMulticast", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 21, Name = "enableThreshold", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 22, Name = "enableRACK", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 23, Name = "enableCircuits", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 24, Name = "enableLocations", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 25, Name = "enableSNMP", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 26, Name = "enablePSNT", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 27, Name = "enableCustomers", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 28, Name = "enableRouting", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 29, Name = "updateTags", Value = false.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 30, Name = "enforceUnique", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 31, Name = "vlanDuplicate", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 32, Name = "decodeMAC", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 33, Name = "enableVaults", Value = true.ToString(), Type = SettingType.Boolean },
                new Setting { Id = 34, Name = "passkeys", Value = true.ToString(), Type = SettingType.Boolean }
            );

        }    
    }
}

