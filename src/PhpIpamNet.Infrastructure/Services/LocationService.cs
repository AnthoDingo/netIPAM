using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class LocationService
{
    private readonly PhpIpamDbContext _db;
    public LocationService(PhpIpamDbContext db) => _db = db;

    public Task<List<Location>> ListAsync(CancellationToken ct = default)
        => _db.Locations.OrderBy(l => l.Name).ToListAsync(ct);

    public Task<Location?> GetAsync(int id, CancellationToken ct = default)
        => _db.Locations.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Location> CreateAsync(Location l, CancellationToken ct = default)
    {
        _db.Locations.Add(l);
        await _db.SaveChangesAsync(ct);
        return l;
    }

    public async Task UpdateAsync(Location l, CancellationToken ct = default)
    {
        _db.Locations.Update(l);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var l = await _db.Locations.FindAsync(new object?[] { id }, ct);
        if (l is null) return;
        _db.Locations.Remove(l);
        await _db.SaveChangesAsync(ct);
    }
}
