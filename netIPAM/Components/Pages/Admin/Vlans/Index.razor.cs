namespace netIPAM.Components.Pages.Admin.Vlans
{
    public partial class Index
    {
        [Inject]
        private VlanService Vlans { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<Vlan>? _vlans;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _vlans = await Vlans.ListAsync();
        private async Task Delete(int id) { await Vlans.DeleteAsync(id); await Reload(); }
    }
}
