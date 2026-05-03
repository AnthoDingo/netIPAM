using static System.Runtime.InteropServices.JavaScript.JSType;

namespace netIPAM.Components.Pages.Admin.Settings
{
    public partial class Index
    {
        [Inject]
        private SettingService SettingsSvc { get; set; } = default!;

        [Inject]
        private MaintenanceState MaintenanceState { get; set; } = default!;

        [Inject]
        private NavigationManager Nav {  get; set; } = default!;

        private Setting? _settings;
        private bool _saved;

        protected override async Task OnInitializedAsync() => _settings = await SettingsSvc.GetOrCreateAsync();

        private async Task Save()
        {
            if (_settings is null) return;
            await SettingsSvc.UpdateAsync(_settings);
            // Synchronise le singleton en mémoire → le middleware prend effet immédiatement
            MaintenanceState.Set(_settings.MaintenanceMode);
            _saved = true;
        }
    }
}
