using Microsoft.EntityFrameworkCore;
using netIPAM.Data;

namespace netIPAM.Services;

public record DashboardStats(int Sections, int Subnets, int IpAddresses, int Vlans, int Vrfs, int Users, int Devices);

public class DashboardService
{
    private readonly AppDbContext _db;
    public DashboardService(AppDbContext db) => _db = db;

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
