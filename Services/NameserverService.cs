using Microsoft.EntityFrameworkCore;
using netIPAM.Entities;
using netIPAM.Data;

namespace netIPAM.Services;

public class NameserverService
{
    private readonly AppDbContext _db;
    public NameserverService(AppDbContext db) => _db = db;

    public Task<List<Nameserver>> ListAsync(CancellationToken ct = default)
        => _db.Nameservers.OrderBy(n => n.Name).ToListAsync(ct);

    public Task<Nameserver?> GetAsync(int id, CancellationToken ct = default)
        => _db.Nameservers.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<Nameserver> CreateAsync(Nameserver n, CancellationToken ct = default)
    {
        n.EditDate = DateTime.UtcNow;
        _db.Nameservers.Add(n);
        await _db.SaveChangesAsync(ct);
        return n;
    }

    public async Task UpdateAsync(Nameserver n, CancellationToken ct = default)
    {
        n.EditDate = DateTime.UtcNow;
        _db.Nameservers.Update(n);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var n = await _db.Nameservers.FindAsync(new object?[] { id }, ct);
        if (n is null) return;
        _db.Nameservers.Remove(n);
        await _db.SaveChangesAsync(ct);
    }
}
