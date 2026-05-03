namespace netIPAM.Components.Pages.Admin.Users
{
    public partial class Index
    {
        [Inject]
        private UserService Users { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<User>? _users;

        protected override async Task OnInitializedAsync() => await Reload();

        private async Task Reload() => _users = await Users.ListAsync();

        private async Task Delete(int id)
        {
            await Users.DeleteAsync(id);
            await Reload();
        }
    }
}
