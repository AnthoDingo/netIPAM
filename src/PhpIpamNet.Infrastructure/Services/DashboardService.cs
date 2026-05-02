using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public record DashboardStats(int Sections, int Subnets, int IpAddresses, int Vlans, int Vrfs, int Users, int Devices);

public class DashboardService
{
    private readonly PhpIpamDbContext _db;
    public DashboardService(PhpIpamDbContext db) => _db = db;

    public async Task<DashboardStats> GetStatsAsync(CancellationToken ct = default)
        => new DashboardStats(
            await _db.Sections.CountAsync(ct),
            await _db.Subnets.CountAsync(ct),
            await _db.IpAddresses.CountAsync(ct),
            await _db.Vlans.CountAsync(ct),
            await _db.Vrfs.CountAsync(ct),
            await _db.Users.CountAsync(ct),
            await _db.Devices.CountAsync(ct));
}
