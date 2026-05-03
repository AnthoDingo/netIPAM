namespace netIPAM.Components.Pages.Admin.AuthMethods
{
    public partial class Index
    {
        [Inject]
        private AuthMethodService Methods { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<UserAuthMethod>? _methods;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _methods = await Methods.ListAsync();
        private async Task Delete(int id) { await Methods.DeleteAsync(id); await Reload(); }
    }
}
