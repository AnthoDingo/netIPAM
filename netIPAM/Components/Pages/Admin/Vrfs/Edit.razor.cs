namespace netIPAM.Components.Pages.Admin.Vrfs
{
    public partial class Edit
    {
        [Inject]
        private VrfService Vrfs { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private Vrf? _vrf;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _vrf = _isNew ? new Vrf() : await Vrfs.GetAsync(Id!.Value);
            if (_vrf is null) Nav.NavigateTo("/admin/vrfs");
        }

        private async Task Save()
        {
            if (_vrf is null) return;
            if (_isNew) await Vrfs.CreateAsync(_vrf); else await Vrfs.UpdateAsync(_vrf);
            Nav.NavigateTo("/admin/vrfs");
        }
    }
}
