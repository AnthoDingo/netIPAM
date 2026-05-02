using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class AuthMethodService
{
    private readonly PhpIpamDbContext _db;
    public AuthMethodService(PhpIpamDbContext db) => _db = db;

    public Task<List<UserAuthMethod>> ListAsync(CancellationToken ct = default)
        => _db.UserAuthMethods.OrderBy(a => a.Id).ToListAsync(ct);

    public Task<UserAuthMethod?> GetAsync(int id, CancellationToken ct = default)
        => _db.UserAuthMethods.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<UserAuthMethod> CreateAsync(UserAuthMethod m, CancellationToken ct = default)
    {
        _db.UserAuthMethods.Add(m);
        await _db.SaveChangesAsync(ct);
        return m;
    }

    public async Task UpdateAsync(UserAuthMethod m, CancellationToken ct = default)
    {
        _db.UserAuthMethods.Update(m);
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Suppression refusée si la méthode est marquée "Protected" (cas des entrées 'local'
    /// et 'http' livrées par le seeder phpIPAM, qui doivent toujours rester présentes).
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var m = await _db.UserAuthMethods.FindAsync(new object?[] { id }, ct);
        if (m is null) return true;
        if (string.Equals(m.Protected, "Yes", StringComparison.OrdinalIgnoreCase))
            return false;
        _db.UserAuthMethods.Remove(m);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
