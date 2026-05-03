namespace netIPAM.Components.Shared
{
    public partial class LanguageSelector : IAsyncDisposable
    {
        [Inject]
        private LocalizationService Localization { get; set; } = default!;

        private string CurrentLanguage => Localization.GetCurrentLanguage();

        protected override async Task OnInitializedAsync()
        {
            Localization.OnLanguageChanged += StateHasChanged;
            await base.OnInitializedAsync();
        }

        private async Task SelectLanguage(string language)
        {
            await Localization.SetLanguageAsync(language);
        }

        private static string GetLanguageName(string language) =>
            LocalizationService.LanguageNames.TryGetValue(language, out var name) ? name : language;

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            Localization.OnLanguageChanged -= StateHasChanged;
            return ValueTask.CompletedTask;
        }
    }
}
