using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Migrations.SqlServer;

/// <summary>
/// Permet à `dotnet ef migrations add ... -p src/PhpIpamNet.Migrations.SqlServer`
/// de fonctionner sans avoir besoin du Web project comme startup.
/// </summary>
public class SqlServerDesignTimeFactory : IDesignTimeDbContextFactory<PhpIpamDbContext>
{
    public PhpIpamDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("PHPIPAM_SQLSERVER")
                   ?? "Server=(localdb)\\MSSQLLocalDB;Database=phpipam_net;Trusted_Connection=True;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<PhpIpamDbContext>()
            .UseSqlServer(conn, b => b.MigrationsAssembly(typeof(SqlServerDesignTimeFactory).Assembly.GetName().Name))
            .Options;

        return new PhpIpamDbContext(options);
    }
}
