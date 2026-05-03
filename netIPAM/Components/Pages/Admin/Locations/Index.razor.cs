namespace netIPAM.Components.Pages.Admin.Locations
{
    public partial class Index : LocalizedComponentBase
    {
        [Inject]
        private LocationService Locations { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<Location>? _locations;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _locations = await Locations.ListAsync();
        private async Task Delete(int id) { await Locations.DeleteAsync(id); await Reload(); }
    }
}
