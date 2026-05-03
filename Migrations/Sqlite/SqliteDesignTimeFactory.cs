using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using netIPAM.Data;

namespace netIPAM.Migrations.Sqlite;

public class SqliteDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string? conn = Environment.GetEnvironmentVariable("PHPIPAM_SQLITE")
                   ?? "Data Source=netipam.db";

        DbContextOptionsBuilder<AppDbContext> options = new();
        options.UseSqlite(conn, b => b.MigrationsAssembly("netIPAM"));

        return new AppDbContext(options.Options);
    }
}
