using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class VlanService
{
    private readonly PhpIpamDbContext _db;
    public VlanService(PhpIpamDbContext db) => _db = db;

    public Task<List<Vlan>> ListAsync(CancellationToken ct = default)
        => _db.Vlans.Include(v => v.Domain).OrderBy(v => v.Number).ToListAsync(ct);

    /// <summary>VLANs utilisés par au moins un subnet dans cette section.</summary>
    public Task<List<Vlan>> ForSectionAsync(int sectionId, CancellationToken ct = default)
        => _db.Vlans
              .Where(v => _db.Subnets.Any(s => s.SectionId == sectionId && s.VlanId == v.VlanId))
              .OrderBy(v => v.Number)
              .ToListAsync(ct);

    public Task<Vlan?> GetAsync(int vlanId, CancellationToken ct = default)
        => _db.Vlans.FirstOrDefaultAsync(v => v.VlanId == vlanId, ct);

    public async Task<Vlan> CreateAsync(Vlan v, CancellationToken ct = default)
    {
        v.EditDate = DateTime.UtcNow;
        _db.Vlans.Add(v);
        await _db.SaveChangesAsync(ct);
        return v;
    }

    public async Task UpdateAsync(Vlan v, CancellationToken ct = default)
    {
        v.EditDate = DateTime.UtcNow;
        _db.Vlans.Update(v);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int vlanId, CancellationToken ct = default)
    {
        var v = await _db.Vlans.FindAsync(new object?[] { vlanId }, ct);
        if (v is null) return;
        _db.Vlans.Remove(v);
        await _db.SaveChangesAsync(ct);
    }
}
