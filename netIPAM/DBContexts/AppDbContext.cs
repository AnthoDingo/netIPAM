using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace netIPAM.DBContexts
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        private readonly IConfiguration _configuration;
        private readonly string _provider;
        private readonly string _connectionString;

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

            //builder.Entity<IdentityUser>(entity => { entity.ToTable("Users"); });
            builder.Entity<AppUser>(entity => { entity.ToTable("Users"); });
            builder.Entity<IdentityRole>(entity => { entity.ToTable("Roles"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });
        }
    }
}
