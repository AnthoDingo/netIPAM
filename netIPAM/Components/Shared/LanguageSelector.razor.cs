namespace netIPAM.Components.Shared
{
    public partial class LanguageSelector
    {
        private string CurrentLanguage => Localization.GetCurrentLanguage();

        private async Task SelectLanguage(string language) =>
            await Localization.SetLanguageAsync(language);

        private static string GetLanguageName(string language) =>
            LocalizationService.LanguageNames.TryGetValue(language, out var name) ? name : language;
    }
}
