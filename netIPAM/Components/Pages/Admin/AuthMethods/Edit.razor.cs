namespace netIPAM.Components.Pages.Admin.AuthMethods
{
    public partial class Edit
    {
        [Inject]
        private AuthMethodService Methods { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private UserAuthMethod? _method;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _method = _isNew ? new UserAuthMethod { Type = "local", Protected = "No" } : await Methods.GetAsync(Id!.Value);
            if (_method is null) Nav.NavigateTo("/admin/authmethods");
        }

        private async Task Save()
        {
            if (_method is null) return;
            if (_isNew) await Methods.CreateAsync(_method); else await Methods.UpdateAsync(_method);
            Nav.NavigateTo("/admin/authmethods");
        }
    }
}
