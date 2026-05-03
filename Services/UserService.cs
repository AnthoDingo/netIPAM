using Microsoft.EntityFrameworkCore;
using netIPAM.Data;
using netIPAM.Identity;
using System.Text.Json;

namespace netIPAM.Services;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;

    public UserService(AppDbContext db, IPasswordHasher hasher)
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
        User? user = await FindByUsernameAsync(username, ct);
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
        User? u = await _db.Users.FindAsync(new object?[] { userId }, ct);
        if (u is null) return;
        // Garde-fou : on ne supprime pas le dernier Administrator
        if (string.Equals(u.Role, "Administrator", StringComparison.OrdinalIgnoreCase))
        {
            int adminCount = await _db.Users.CountAsync(x => x.Role == "Administrator", ct);
            if (adminCount <= 1) return;
        }
        _db.Users.Remove(u);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ResetPasswordAsync(int userId, string newPassword, CancellationToken ct = default)
    {
        User? user = await _db.Users.FindAsync(new object?[] { userId }, ct);
        if (user is null) return;
        user.Password = _hasher.Hash(newPassword);
        user.EditDate = DateTime.UtcNow;
        user.PassChange = "No";
        await _db.SaveChangesAsync(ct);
    }

    // ── Gestion des appartenances aux groupes ──────────────────────────

    /// <summary>
    /// Retourne les IDs de groupes dont fait partie l'utilisateur.
    /// Format phpIPAM : JSON {"g_id":"level"} — on extrait les clés.
    /// </summary>
    public List<int> GetGroupIds(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Groups)) return [];
        try
        {
            Dictionary<string, string>? dict = JsonSerializer.Deserialize<Dictionary<string, string>>(user.Groups);
            return dict?.Keys
                .Select(k => int.TryParse(k, out var id) ? id : -1)
                .Where(id => id > 0)
                .ToList() ?? [];
        }
        catch { return []; }
    }

    /// <summary>
    /// Persiste les IDs de groupes pour un utilisateur.
    /// Conserve le level=1 (read) par défaut pour les nouveaux groupes.
    /// </summary>
    public async Task SetGroupIdsAsync(int userId, List<int> groupIds, CancellationToken ct = default)
    {
        User? user = await _db.Users.FindAsync(new object?[] { userId }, ct);
        if (user is null) return;

        // Reconstruire le JSON en préservant les levels existants
        Dictionary<string, string> existing = new();
        if (!string.IsNullOrWhiteSpace(user.Groups))
        {
            try { existing = JsonSerializer.Deserialize<Dictionary<string, string>>(user.Groups) ?? []; }
            catch { /* ignore format invalide */ }
        }

        Dictionary<string, string> newDict = new();
        foreach (int gId in groupIds)
        {
            string key = gId.ToString();
            newDict[key] = existing.TryGetValue(key, out var level) ? level : "1";
        }

        user.Groups = newDict.Count > 0 ? JsonSerializer.Serialize(newDict) : null;
        user.EditDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Retourne tous les utilisateurs appartenant à un groupe donné.</summary>
    public async Task<List<User>> GetMembersOfGroupAsync(int groupId, CancellationToken ct = default)
    {
        List<User> all = await _db.Users.ToListAsync(ct);
        string gKey = $"\"{groupId}\"";
        // On cherche la clé dans le JSON sans désérialiser en masse
        return all.Where(u => u.Groups != null && u.Groups.Contains(gKey)).ToList();
    }
}

