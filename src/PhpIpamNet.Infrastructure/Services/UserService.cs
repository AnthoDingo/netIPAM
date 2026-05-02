using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;
using PhpIpamNet.Infrastructure.Identity;

namespace PhpIpamNet.Infrastructure.Services;

public class UserService
{
    private readonly PhpIpamDbContext _db;
    private readonly IPasswordHasher _hasher;

    public UserService(PhpIpamDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public Task<List<User>> ListAsync(CancellationToken ct = default)
        => _db.Users.OrderBy(u => u.Username).ToListAsync(ct);

    public Task<User?> FindByUsernameAsync(string username, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<User?> FindByIdAsync(int id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    /// <summary>
    /// Authentifie un utilisateur local. Met à jour LastLogin/LastActivity au succès.
    /// Retourne null si l'identifiant est invalide ou le compte désactivé.
    /// </summary>
    public async Task<User?> AuthenticateLocalAsync(string username, string password, CancellationToken ct = default)
    {
        var user = await FindByUsernameAsync(username, ct);
        if (user is null) return null;
        if (string.Equals(user.Disabled, "Yes", StringComparison.OrdinalIgnoreCase)) return null;
        if (string.IsNullOrEmpty(user.Password)) return null;
        if (!_hasher.Verify(password, user.Password)) return null;

        user.LastLogin = DateTime.UtcNow;
        user.LastActivity = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<User> CreateAsync(User user, string password, CancellationToken ct = default)
    {
        user.Password = _hasher.Hash(password);
        user.EditDate = DateTime.UtcNow;
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }

    /// <summary>Mise à jour d'un user sans modifier le mot de passe.</summary>
    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        user.EditDate = DateTime.UtcNow;
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int userId, CancellationToken ct = default)
    {
        var u = await _db.Users.FindAsync(new object?[] { userId }, ct);
        if (u is null) return;
        // Garde-fou : on ne supprime pas le dernier Administrator
        if (string.Equals(u.Role, "Administrator", StringComparison.OrdinalIgnoreCase))
        {
            var adminCount = await _db.Users.CountAsync(x => x.Role == "Administrator", ct);
            if (adminCount <= 1) return;
        }
        _db.Users.Remove(u);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ResetPasswordAsync(int userId, string newPassword, CancellationToken ct = default)
    {
        var user = await _db.Users.FindAsync(new object?[] { userId }, ct);
        if (user is null) return;
        user.Password = _hasher.Hash(newPassword);
        user.EditDate = DateTime.UtcNow;
        user.PassChange = "No";
        await _db.SaveChangesAsync(ct);
    }
}
