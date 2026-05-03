using Microsoft.EntityFrameworkCore;
using netIPAM.Entities;
using netIPAM.Data;

namespace netIPAM.Services;

public class VlanDomainService
{
    private readonly AppDbContext _db;
    public VlanDomainService(AppDbContext db) => _db = db;

    public Task<List<VlanDomain>> ListAsync(CancellationToken ct = default)
        => _db.VlanDomains.OrderBy(d => d.Name).ToListAsync(ct);

    public Task<VlanDomain?> GetAsync(int id, CancellationToken ct = default)
        => _db.VlanDomains.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<VlanDomain> CreateAsync(VlanDomain d, CancellationToken ct = default)
    {
        _db.VlanDomains.Add(d);
        await _db.SaveChangesAsync(ct);
        return d;
    }

    public async Task UpdateAsync(VlanDomain d, CancellationToken ct = default)
    {
        _db.VlanDomains.Update(d);
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Refus si le domaine 1 (default) est cible de suppression — il sert d'ancre.</summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (id == 1) return false;
        VlanDomains? d = await _db.VlanDomains.FindAsync(new object?[] { id }, ct);
        if (d is null) return true;
        _db.VlanDomains.Remove(d);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
