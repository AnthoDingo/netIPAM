namespace netIPAM.Components.Pages.Admin.Groups
{
    public partial class Index
    {
        [Inject]
        private UserGroupService Groups { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<UserGroup>? _groups;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _groups = await Groups.ListAsync();
        private async Task Delete(int id) { await Groups.DeleteAsync(id); await Reload(); }
    }
}
