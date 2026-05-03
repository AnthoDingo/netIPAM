namespace netIPAM.Components.Pages.Admin.Locations
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private LocationService Locations { get; set; } = default!;
        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private Location? _loc;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _loc = _isNew ? new Location() : await Locations.GetAsync(Id!.Value);
            if (_loc is null) Nav.NavigateTo("/admin/locations");
        }

        private async Task Save()
        {
            if (_loc is null) return;
            if (_isNew) await Locations.CreateAsync(_loc); else await Locations.UpdateAsync(_loc);
            Nav.NavigateTo("/admin/locations");
        }
    }
}
