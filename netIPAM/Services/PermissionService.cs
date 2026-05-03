using Microsoft.EntityFrameworkCore;
using netIPAM.Data;

namespace netIPAM.Services;

/// <summary>
/// Gestion et résolution des permissions sur les ressources réseau.
///
/// Modèle : un sujet (user/group) a un niveau 0-3 sur chaque ressource.
/// Résolution effective pour un utilisateur = max(permissions directes, permissions de ses groupes).
/// </summary>
public class PermissionService
{
    private readonly AppDbContext _db;

    public PermissionService(AppDbContext db) => _db = db;

    // ──────────────────────────────────────────────────────────────
    // Lecture
    // ──────────────────────────────────────────────────────────────

    /// <summary>Toutes les permissions d'un sujet pour un type de ressource.</summary>
    public Task<List<EntityPermission>> GetForSubjectAsync(
        string subjectType, int subjectId, string entityType, CancellationToken ct = default)
        => _db.EntityPermissions
              .Where(p => p.SubjectType == subjectType
                       && p.SubjectId == subjectId
                       && p.EntityType == entityType)
              .ToListAsync(ct);

    /// <summary>Toutes les permissions d'un sujet (tous types de ressources).</summary>
    public Task<List<EntityPermission>> GetAllForSubjectAsync(
        string subjectType, int subjectId, CancellationToken ct = default)
        => _db.EntityPermissions
              .Where(p => p.SubjectType == subjectType && p.SubjectId == subjectId)
              .ToListAsync(ct);

    /// <summary>Tous les sujets ayant une permission sur une ressource donnée.</summary>
    public Task<List<EntityPermission>> GetForEntityAsync(
        string entityType, int entityId, CancellationToken ct = default)
        => _db.EntityPermissions
              .Where(p => p.EntityType == entityType && p.EntityId == entityId && p.Level > 0)
              .ToListAsync(ct);

    // ──────────────────────────────────────────────────────────────
    // Écriture
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Pose ou met à jour le niveau d'un sujet sur une ressource.
    /// Si level == 0 : supprime la ligne (aucun accès = absence de ligne).
    /// </summary>
    public async Task SetAsync(
        string subjectType, int subjectId,
        string entityType, int entityId,
        int level,
        CancellationToken ct = default)
    {
        EntityPermission? existing = await _db.EntityPermissions
            .FirstOrDefaultAsync(p => p.SubjectType == subjectType
                                   && p.SubjectId == subjectId
                                   && p.EntityType == entityType
                                   && p.EntityId == entityId, ct);

        if (level == PermissionLevels.None)
        {
            if (existing is not null) _db.EntityPermissions.Remove(existing);
        }
        else if (existing is null)
        {
            _db.EntityPermissions.Add(new EntityPermission
            {
                SubjectType = subjectType,
                SubjectId = subjectId,
                EntityType = entityType,
                EntityId = entityId,
                Level = level,
                CreatedAt = DateTime.UtcNow,
            });
        }
        else
        {
            existing.Level = level;
        }

        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Remplace en bloc toutes les permissions d'un sujet pour un type de ressource.
    /// permissions : liste de (entityId, level). Les ressources absentes → level 0.
    /// </summary>
    public async Task SetBulkAsync(
        string subjectType, int subjectId,
        string entityType,
        IEnumerable<(int entityId, int level)> permissions,
        CancellationToken ct = default)
    {
        // Supprimer les existantes pour ce sujet+type
        List<EntityPermission> existing = await _db.EntityPermissions
            .Where(p => p.SubjectType == subjectType
                     && p.SubjectId == subjectId
                     && p.EntityType == entityType)
            .ToListAsync(ct);
        _db.EntityPermissions.RemoveRange(existing);

        // Insérer les nouvelles (level > 0 uniquement)
        foreach (var (entityId, level) in permissions.Where(p => p.level > PermissionLevels.None))
        {
            _db.EntityPermissions.Add(new EntityPermission
            {
                SubjectType = subjectType,
                SubjectId = subjectId,
                EntityType = entityType,
                EntityId = entityId,
                Level = level,
                CreatedAt = DateTime.UtcNow,
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Supprime toutes les permissions d'un sujet (ex : suppression d'un user/groupe).</summary>
    public async Task DeleteAllForSubjectAsync(
        string subjectType, int subjectId, CancellationToken ct = default)
    {
        List<EntityPermission> rows = await _db.EntityPermissions
            .Where(p => p.SubjectType == subjectType && p.SubjectId == subjectId)
            .ToListAsync(ct);
        _db.EntityPermissions.RemoveRange(rows);
        await _db.SaveChangesAsync(ct);
    }

    // ──────────────────────────────────────────────────────────────
    // Résolution effective (user + ses groupes)
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Construit un dictionnaire entityId → level effectif pour un utilisateur donné,
    /// en prenant le MAX entre ses permissions directes et celles de ses groupes.
    /// </summary>
    public async Task<Dictionary<int, int>> GetEffectiveAsync(
        int userId,
        List<int> groupIds,
        string entityType,
        CancellationToken ct = default)
    {
        Dictionary<int, int> result = new();

        // Permissions directes
        List<EntityPermission> direct = await _db.EntityPermissions
            .Where(p => p.SubjectType == SubjectTypes.User
                     && p.SubjectId == userId
                     && p.EntityType == entityType)
            .ToListAsync(ct);

        foreach (EntityPermission p in direct)
            result[p.EntityId] = p.Level;

        // Permissions des groupes (prendre le max)
        if (groupIds.Count > 0)
        {
            List<EntityPermission> groupPerms = await _db.EntityPermissions
                .Where(p => p.SubjectType == SubjectTypes.Group
                         && groupIds.Contains(p.SubjectId)
                         && p.EntityType == entityType)
                .ToListAsync(ct);

            foreach (EntityPermission p in groupPerms)
            {
                if (!result.TryGetValue(p.EntityId, out var cur) || p.Level > cur)
                    result[p.EntityId] = p.Level;
            }
        }

        return result;
    }

    /// <summary>Résumé : nombre de ressources avec accès > 0 par type, pour un sujet.</summary>
    public async Task<Dictionary<string, int>> GetPermissionCountsAsync(
        string subjectType, int subjectId, CancellationToken ct = default)
    {
        var perms = await _db.EntityPermissions
            .Where(p => p.SubjectType == subjectType && p.SubjectId == subjectId && p.Level > 0)
            .GroupBy(p => p.EntityType)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return perms.ToDictionary(x => x.Key, x => x.Count);
    }
}
