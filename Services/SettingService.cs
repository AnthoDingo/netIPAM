using Microsoft.EntityFrameworkCore;
using netIPAM.Entities;
using netIPAM.Data;

namespace netIPAM.Services;

/// <summary>
/// La table `settings` est un singleton (une seule ligne, id = 1) en phpIPAM. Get/Update.
/// </summary>
public class SettingService
{
    private readonly AppDbContext _db;
    public SettingService(AppDbContext db) => _db = db;

    public async Task<Setting> GetOrCreateAsync(CancellationToken ct = default)
    {
        Settings? s = await _db.Settings.FirstOrDefaultAsync(ct);
        if (s is null)
        {
            s = new Setting { SiteTitle = "phpipam IP address management", Theme = "dark" };
            _db.Settings.Add(s);
            await _db.SaveChangesAsync(ct);
        }
        return s;
    }

    public async Task UpdateAsync(Setting s, CancellationToken ct = default)
    {
        s.EditDate = DateTime.UtcNow;
        _db.Settings.Update(s);
        await _db.SaveChangesAsync(ct);
    }
}
