using Microsoft.EntityFrameworkCore;
using netIPAM.Entities;
using netIPAM.Data;

namespace netIPAM.Services;

public class DeviceService
{
    private readonly AppDbContext _db;
    public DeviceService(AppDbContext db) => _db = db;

    public Task<List<Device>> ListAsync(CancellationToken ct = default)
        => _db.Devices.OrderBy(d => d.Hostname).ToListAsync(ct);

    public Task<Device?> GetAsync(int id, CancellationToken ct = default)
        => _db.Devices.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<Device> CreateAsync(Device d, CancellationToken ct = default)
    {
        d.EditDate = DateTime.UtcNow;
        _db.Devices.Add(d);
        await _db.SaveChangesAsync(ct);
        return d;
    }

    public async Task UpdateAsync(Device d, CancellationToken ct = default)
    {
        d.EditDate = DateTime.UtcNow;
        _db.Devices.Update(d);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Devices? d = await _db.Devices.FindAsync(new object?[] { id }, ct);
        if (d is null) return;
        _db.Devices.Remove(d);
        await _db.SaveChangesAsync(ct);
    }
}
