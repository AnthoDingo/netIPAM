namespace netIPAM.Components.Pages.Admin.DeviceTypes
{
    public partial class Index : LocalizedComponentBase
    {
        [Inject]
        private DeviceTypeService Types { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<DeviceType>? _types;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _types = await Types.ListAsync();
        private async Task Delete(int id) { await Types.DeleteAsync(id); await Reload(); }
    }
}
