using Microsoft.EntityFrameworkCore;
using netIPAM.Entities;
using netIPAM.Data;

namespace netIPAM.Services;

public class DeviceTypeService
{
    private readonly AppDbContext _db;
    public DeviceTypeService(AppDbContext db) => _db = db;

    public Task<List<DeviceType>> ListAsync(CancellationToken ct = default)
        => _db.DeviceTypes.OrderBy(t => t.Tname).ToListAsync(ct);

    public Task<DeviceType?> GetAsync(int tid, CancellationToken ct = default)
        => _db.DeviceTypes.FirstOrDefaultAsync(t => t.Tid == tid, ct);

    public async Task<DeviceType> CreateAsync(DeviceType t, CancellationToken ct = default)
    {
        _db.DeviceTypes.Add(t);
        await _db.SaveChangesAsync(ct);
        return t;
    }

    public async Task UpdateAsync(DeviceType t, CancellationToken ct = default)
    {
        _db.DeviceTypes.Update(t);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int tid, CancellationToken ct = default)
    {
        var t = await _db.DeviceTypes.FindAsync(new object?[] { tid }, ct);
        if (t is null) return;
        _db.DeviceTypes.Remove(t);
        await _db.SaveChangesAsync(ct);
    }
}
