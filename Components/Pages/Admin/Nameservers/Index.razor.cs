namespace netIPAM.Components.Pages.Admin.Nameservers
{
    public partial class Index
    {
        [Inject]
        private NameserverService Nameservers { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<Nameserver>? _nameservers;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _nameservers = await Nameservers.ListAsync();
        private async Task Delete(int id) { await Nameservers.DeleteAsync(id); await Reload(); }
    }
}
