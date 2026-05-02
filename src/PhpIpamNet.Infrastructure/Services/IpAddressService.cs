using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class IpAddressService
{
    private readonly PhpIpamDbContext _db;
    public IpAddressService(PhpIpamDbContext db) => _db = db;

    public Task<List<IpAddress>> ForSubnetAsync(int subnetId, CancellationToken ct = default)
        => _db.IpAddresses
              .Where(i => i.SubnetId == subnetId)
              .OrderBy(i => i.IpAddr)
              .ToListAsync(ct);

    public Task<IpAddress?> GetAsync(int id, CancellationToken ct = default)
        => _db.IpAddresses.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IpAddress> CreateAsync(IpAddress ip, CancellationToken ct = default)
    {
        ip.EditDate = DateTime.UtcNow;
        _db.IpAddresses.Add(ip);
        await _db.SaveChangesAsync(ct);
        return ip;
    }

    public async Task UpdateAsync(IpAddress ip, CancellationToken ct = default)
    {
        ip.EditDate = DateTime.UtcNow;
        _db.IpAddresses.Update(ip);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var ip = await _db.IpAddresses.FindAsync(new object?[] { id }, ct);
        if (ip is null) return;
        _db.IpAddresses.Remove(ip);
        await _db.SaveChangesAsync(ct);
    }
}
