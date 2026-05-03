namespace netIPAM.Components.Pages.Admin.Nameservers
{
    public partial class Edit
    {
        [Inject]
        private NameserverService Nameservers { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private Nameserver? _ns;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _ns = _isNew ? new Nameserver() : await Nameservers.GetAsync(Id!.Value);
            if (_ns is null) Nav.NavigateTo("/admin/nameservers");
        }

        private async Task Save()
        {
            if (_ns is null) return;
            if (_isNew) await Nameservers.CreateAsync(_ns); else await Nameservers.UpdateAsync(_ns);
            Nav.NavigateTo("/admin/nameservers");
        }
    }
}
