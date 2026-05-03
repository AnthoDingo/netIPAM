namespace netIPAM.Components.Pages.Admin.VlanDomains
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private VlanDomainService Domains { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private VlanDomain? _domain;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _domain = _isNew ? new VlanDomain() : await Domains.GetAsync(Id!.Value);
            if (_domain is null) Nav.NavigateTo("/admin/l2domains");
        }

        private async Task Save()
        {
            if (_domain is null) return;
            if (_isNew) await Domains.CreateAsync(_domain); else await Domains.UpdateAsync(_domain);
            Nav.NavigateTo("/admin/l2domains");
        }
    }
}
