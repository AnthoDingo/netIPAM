using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using netIPAM.DBContexts;

namespace netIPAM.Services
{
    public class CacheService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly IMemoryCache _cache;

        public CacheService(IDbContextFactory<AppDbContext> dbFactory, IMemoryCache cache)
        {
            _dbFactory = dbFactory;
            _cache = cache;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            return await _cache.GetOrCreateAsync("app_settings", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                using AppDbContext db = _dbFactory.CreateDbContext();
                return await db.Settings.ToDictionaryAsync(s => s.Name, s => s.Value ?? "");
            });
        }

        public async Task<string> GetSettingValueAsync(string key, string defaultValue = "")
        {
            Dictionary<string, string> settings = await GetSettingsAsync();
            return settings.GetValueOrDefault(key, defaultValue);
        }

        public async Task<bool> GetSettingBoolAsync(string key) => bool.TryParse(await GetSettingValueAsync(key, "false"), out bool v) && v;

        public string GetString(Dictionary<string, string> dictionnary, string key, string defaultValue = "") =>
            dictionnary.GetValueOrDefault(key, defaultValue);

        public bool GetBool(Dictionary<string, string> dictionnary, string key) =>
            bool.TryParse(dictionnary.GetValueOrDefault(key, "false"), out bool v) && v;

        public int GetInt(Dictionary<string, string> dictionnary, string key, int defaultValue = 0) =>
            int.TryParse(dictionnary.GetValueOrDefault(key, ""), out int v) ? v : defaultValue;

        public void InvalidateCache(string key) => _cache.Remove(key);

    }
}
