using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Migrations.Sqlite;

public class SqliteDesignTimeFactory : IDesignTimeDbContextFactory<PhpIpamDbContext>
{
    public PhpIpamDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("PHPIPAM_SQLITE")
                   ?? "Data Source=phpipam_net.db";

        var options = new DbContextOptionsBuilder<PhpIpamDbContext>()
            .UseSqlite(conn, b => b.MigrationsAssembly(typeof(SqliteDesignTimeFactory).Assembly.GetName().Name))
            .Options;

        return new PhpIpamDbContext(options);
    }
}
