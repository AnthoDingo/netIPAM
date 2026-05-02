using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class IpTagService
{
    private readonly PhpIpamDbContext _db;
    public IpTagService(PhpIpamDbContext db) => _db = db;

    public Task<List<IpTag>> ListAsync(CancellationToken ct = default)
        => _db.IpTags.OrderBy(t => t.Id).ToListAsync(ct);

    public Task<IpTag?> GetAsync(int id, CancellationToken ct = default)
        => _db.IpTags.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IpTag> CreateAsync(IpTag t, CancellationToken ct = default)
    {
        _db.IpTags.Add(t);
        await _db.SaveChangesAsync(ct);
        return t;
    }

    public async Task UpdateAsync(IpTag t, CancellationToken ct = default)
    {
        _db.IpTags.Update(t);
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Les tags marqués Locked = "Yes" (cas des 4 tags système Offline/Used/Reserved/DHCP) ne sont pas supprimables.</summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var t = await _db.IpTags.FindAsync(new object?[] { id }, ct);
        if (t is null) return true;
        if (string.Equals(t.Locked, "Yes", StringComparison.OrdinalIgnoreCase)) return false;
        _db.IpTags.Remove(t);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
