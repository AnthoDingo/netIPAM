namespace netIPAM.Components.Pages.Admin.Vrfs
{
    public partial class Index
    {
        [Inject]
        private VrfService Vrfs { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<Vrf>? _vrfs;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _vrfs = await Vrfs.ListAsync();
        private async Task Delete(int id) { await Vrfs.DeleteAsync(id); await Reload(); }
    }
}
