using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace netIPAM.Services
{
    /// <summary>
    /// Service de localisation pour Blazor Server.
    /// Lit les fichiers JSON depuis wwwroot/i18n/{lang}/translation.json.
    /// Scoped par circuit SignalR — chaque onglet/utilisateur a sa propre instance.
    /// </summary>
    public class LocalizationService
    {
        private readonly IWebHostEnvironment _env;

        private Dictionary<string, JsonElement> _translations = new();
        private string _currentLanguage = "en";
        private bool _initialized = false;

        public static readonly string[] AvailableLanguages = ["en", "fr", "it", "es", "de"];

        public static readonly Dictionary<string, string> LanguageNames = new()
        {
            ["en"] = "English",
            ["fr"] = "Français",
            ["it"] = "Italiano",
            ["es"] = "Español",
            ["de"] = "Deutsch",
        };

        /// <summary>Notifie les composants abonnés d'un changement de langue.</summary>
        public event Action? OnLanguageChanged;

        public LocalizationService(IWebHostEnvironment env)
        {
            _env = env;
        }

        // ── API publique ───────────────────────────────────────────────────────

        public string GetCurrentLanguage() => _currentLanguage;

        public async Task SetLanguageAsync(string language)
        {
            if (!AvailableLanguages.Contains(language))
                language = "en";

            if (_initialized && _currentLanguage == language)
                return;

            _currentLanguage = language;
            LoadTranslations(language);
            _initialized = true;

            OnLanguageChanged?.Invoke();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Retourne la traduction pour la clé "section.key".
        /// Retourne la clé elle-même si introuvable (jamais null, jamais d'exception).
        /// </summary>
        public string Get(string key)
        {
            if (!_initialized)
                LoadTranslations(_currentLanguage);

            if (string.IsNullOrWhiteSpace(key))
                return key;

            var parts = key.Split('.', 2);
            if (parts.Length < 2)
                return key;

            if (!_translations.TryGetValue(parts[0], out var section))
                return key;

            if (section.TryGetProperty(parts[1], out var value))
                return value.GetString() ?? key;

            return key;
        }

        // ── Lecture des fichiers ───────────────────────────────────────────────

        private void LoadTranslations(string language)
        {
            _translations.Clear();

            var path = Path.Combine(
                _env.WebRootPath,
                "i18n", language, "translation.json");

            if (!File.Exists(path))
            {
                // Fallback vers l'anglais
                if (language != "en")
                    LoadTranslations("en");
                return;
            }

            try
            {
                var json = File.ReadAllText(path);
                using var doc = JsonDocument.Parse(json);

                foreach (var section in doc.RootElement.EnumerateObject())
                    _translations[section.Name] = section.Value.Clone();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[LocalizationService] Error loading {language}: {ex.Message}");
            }
        }
    }
}
