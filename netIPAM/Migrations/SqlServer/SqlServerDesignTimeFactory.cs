using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using netIPAM.Data;

namespace netIPAM.Migrations.SqlServer;

/// <summary>
/// Permet à `dotnet ef migrations add ... -p src/netIPAM.Migrations.SqlServer`
/// de fonctionner sans avoir besoin du Web project comme startup.
/// </summary>
public class SqlServerDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string? conn = Environment.GetEnvironmentVariable("PHPIPAM_SQLSERVER")
                   ?? "Server=(localdb)\\MSSQLLocalDB;Database=netipam;Trusted_Connection=True;TrustServerCertificate=True";

        DbContextOptionsBuilder<AppDbContext> options = new();
        options.UseSqlServer(conn, b => b.MigrationsAssembly("netIPAM"));

        return new AppDbContext(options.Options);
    }
}
