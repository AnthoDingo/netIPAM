using Microsoft.EntityFrameworkCore;
using netIPAM.Entities;
using netIPAM.Data;

namespace netIPAM.Services;

public class VrfService
{
    private readonly AppDbContext _db;
    public VrfService(AppDbContext db) => _db = db;

    public Task<List<Vrf>> ListAsync(CancellationToken ct = default)
        => _db.Vrfs.OrderBy(v => v.Name).ToListAsync(ct);

    public Task<Vrf?> GetAsync(int vrfId, CancellationToken ct = default)
        => _db.Vrfs.FirstOrDefaultAsync(v => v.VrfId == vrfId, ct);

    public async Task<Vrf> CreateAsync(Vrf v, CancellationToken ct = default)
    {
        v.EditDate = DateTime.UtcNow;
        _db.Vrfs.Add(v);
        await _db.SaveChangesAsync(ct);
        return v;
    }

    public async Task UpdateAsync(Vrf v, CancellationToken ct = default)
    {
        v.EditDate = DateTime.UtcNow;
        _db.Vrfs.Update(v);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int vrfId, CancellationToken ct = default)
    {
        Vrfs? v = await _db.Vrfs.FindAsync(new object?[] { vrfId }, ct);
        if (v is null) return;
        _db.Vrfs.Remove(v);
        await _db.SaveChangesAsync(ct);
    }
}
