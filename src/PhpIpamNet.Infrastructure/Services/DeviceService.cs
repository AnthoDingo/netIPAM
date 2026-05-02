using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class DeviceService
{
    private readonly PhpIpamDbContext _db;
    public DeviceService(PhpIpamDbContext db) => _db = db;

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
        var d = await _db.Devices.FindAsync(new object?[] { id }, ct);
        if (d is null) return;
        _db.Devices.Remove(d);
        await _db.SaveChangesAsync(ct);
    }
}
