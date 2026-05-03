namespace netIPAM.Components.Pages.Admin.Vlans
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private VlanService Vlans { get; set; } = default!;

        [Inject]
        private VlanDomainService Domains { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private Vlan? _vlan;
        private List<VlanDomain>? _domains;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _domains = await Domains.ListAsync();
            _vlan = _isNew ? new Vlan { DomainId = 1 } : await Vlans.GetAsync(Id!.Value);
            if (_vlan is null) Nav.NavigateTo("/admin/vlans");
        }

        private async Task Save()
        {
            if (_vlan is null) return;
            if (_isNew) await Vlans.CreateAsync(_vlan); else await Vlans.UpdateAsync(_vlan);
            Nav.NavigateTo("/admin/vlans");
        }
    }
}
