using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using netIPAM.Data;

namespace netIPAM.Migrations.Sqlite;

public class SqliteDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string? conn = Environment.GetEnvironmentVariable("PHPIPAM_SQLITE")
                   ?? "Data Source=phpipam_net.db";

        DbContextOptionsBuilder<AppDbContext> options = new()
            .UseSqlite(conn, b => b.MigrationsAssembly("netIPAM"))
            .Options;

        return new AppDbContext(options);
    }
}
