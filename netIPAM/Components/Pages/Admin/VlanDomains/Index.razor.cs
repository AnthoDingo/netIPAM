namespace netIPAM.Components.Pages.Admin.VlanDomains
{
    public partial class Index
    {
        [Inject]
        private VlanDomainService Domains { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<VlanDomain>? _domains;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _domains = await Domains.ListAsync();
        private async Task Delete(int id) { await Domains.DeleteAsync(id); await Reload(); }
    }
}
