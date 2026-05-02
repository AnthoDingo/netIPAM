using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class SectionService
{
    private readonly PhpIpamDbContext _db;
    public SectionService(PhpIpamDbContext db) => _db = db;

    public Task<List<Section>> ListAsync(CancellationToken ct = default)
        => _db.Sections.OrderBy(s => s.Order).ThenBy(s => s.Name).ToListAsync(ct);

    public Task<Section?> GetAsync(int id, CancellationToken ct = default)
        => _db.Sections.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Section> CreateAsync(Section section, CancellationToken ct = default)
    {
        section.EditDate = DateTime.UtcNow;
        _db.Sections.Add(section);
        await _db.SaveChangesAsync(ct);
        return section;
    }

    public async Task UpdateAsync(Section section, CancellationToken ct = default)
    {
        section.EditDate = DateTime.UtcNow;
        _db.Sections.Update(section);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var s = await _db.Sections.FindAsync(new object?[] { id }, ct);
        if (s is null) return;
        _db.Sections.Remove(s);
        await _db.SaveChangesAsync(ct);
    }
}
