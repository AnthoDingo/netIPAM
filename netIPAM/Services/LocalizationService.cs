using System.Globalization;
using System.Text.Json;

namespace netIPAM.Services
{
    /// <summary>
    /// Service de localisation pour gérer les traductions multilingues
    /// </summary>
    public class LocalizationService
    {
        private readonly HttpClient _httpClient;
        private Dictionary<string, JsonElement> _translations = new();
        private string _currentLanguage = "en";
        
        // Langues disponibles
        public static readonly string[] AvailableLanguages = { "en", "fr", "it", "es", "de" };
        public static readonly Dictionary<string, string> LanguageNames = new()
        {
            { "en", "English" },
            { "fr", "Français" },
            { "it", "Italiano" },
            { "es", "Español" },
            { "de", "Deutsch" }
        };

        public event Action? OnLanguageChanged;

        public LocalizationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Initialise le service avec la langue par défaut
        /// </summary>
        public async Task InitializeAsync(string language = "en")
        {
            // Valider la langue
            if (!AvailableLanguages.Contains(language))
            {
                language = "en";
            }

            await SetLanguageAsync(language);
        }

        /// <summary>
        /// Change la langue actuelle
        /// </summary>
        public async Task SetLanguageAsync(string language)
        {
            if (!AvailableLanguages.Contains(language))
            {
                throw new ArgumentException($"Language '{language}' is not supported");
            }

            if (_currentLanguage == language && _translations.Count > 0)
            {
                return; // Déjà chargée
            }

            _currentLanguage = language;
            
            // Charger les traductions
            await LoadTranslationsAsync(language);
            
            // Mettre à jour la culture globale
            var cultureInfo = new CultureInfo(language);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            
            OnLanguageChanged?.Invoke();
        }

        /// <summary>
        /// Charge les traductions depuis le fichier JSON
        /// </summary>
        private async Task LoadTranslationsAsync(string language)
        {
            try
            {
                var response = await _httpClient.GetAsync($"i18n/{language}/translation.json");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                
                _translations.Clear();
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    _translations[prop.Name] = prop.Value.Clone();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading translations for language '{language}': {ex.Message}");
                _translations.Clear();
            }
        }

        /// <summary>
        /// Obtient une traduction par clé
        /// </summary>
        public string Get(string key)
        {
            return Get(key, key); // Retourner la clé si traduction non trouvée
        }

        /// <summary>
        /// Obtient une traduction par clé avec valeur par défaut
        /// </summary>
        public string Get(string key, string defaultValue)
        {
            try
            {
                var parts = key.Split('.');
                if (parts.Length < 2)
                {
                    return defaultValue;
                }

                if (!_translations.TryGetValue(parts[0], out var section))
                {
                    return defaultValue;
                }

                var current = section;
                for (int i = 1; i < parts.Length; i++)
                {
                    if (current.ValueKind != JsonValueKind.Object)
                    {
                        return defaultValue;
                    }

                    if (!current.TryGetProperty(parts[i], out var next))
                    {
                        return defaultValue;
                    }

                    current = next;
                }

                return current.GetString() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Obtient la langue actuelle
        /// </summary>
        public string GetCurrentLanguage() => _currentLanguage;

        /// <summary>
        /// Obtient le nom de la langue actuelle
        /// </summary>
        public string GetCurrentLanguageName() => 
            LanguageNames.TryGetValue(_currentLanguage, out var name) ? name : _currentLanguage;

        /// <summary>
        /// Obtient toutes les langues disponibles
        /// </summary>
        public string[] GetAvailableLanguages() => AvailableLanguages;
    }
}
