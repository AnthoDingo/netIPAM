using Microsoft.EntityFrameworkCore;
using netIPAM.Data;

namespace netIPAM.Services;

public class UserGroupService
{
    private readonly AppDbContext _db;
    public UserGroupService(AppDbContext db) => _db = db;

    public Task<List<UserGroup>> ListAsync(CancellationToken ct = default)
        => _db.UserGroups.OrderBy(g => g.GName).ToListAsync(ct);

    public Task<UserGroup?> GetAsync(int id, CancellationToken ct = default)
        => _db.UserGroups.FirstOrDefaultAsync(g => g.GId == id, ct);

    public async Task<UserGroup> CreateAsync(UserGroup g, CancellationToken ct = default)
    {
        g.EditDate = DateTime.UtcNow;
        _db.UserGroups.Add(g);
        await _db.SaveChangesAsync(ct);
        return g;
    }

    public async Task UpdateAsync(UserGroup g, CancellationToken ct = default)
    {
        g.EditDate = DateTime.UtcNow;
        _db.UserGroups.Update(g);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        UserGroup? g = await _db.UserGroups.FindAsync(new object?[] { id }, ct);
        if (g is null) return;
        _db.UserGroups.Remove(g);
        await _db.SaveChangesAsync(ct);
    }
}
