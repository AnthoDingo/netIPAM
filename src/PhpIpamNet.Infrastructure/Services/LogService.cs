using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class LogService
{
    private readonly PhpIpamDbContext _db;
    public LogService(PhpIpamDbContext db) => _db = db;

    /// <summary>Logs récents avec pagination simple côté serveur (limit 200 par défaut).</summary>
    public Task<List<Log>> ListAsync(int take = 200, int? severityFilter = null, CancellationToken ct = default)
    {
        var q = _db.Logs.AsQueryable();
        if (severityFilter is int s) q = q.Where(l => l.Severity == s);
        return q.OrderByDescending(l => l.Id).Take(take).ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default) => await _db.Logs.CountAsync(ct);
}
