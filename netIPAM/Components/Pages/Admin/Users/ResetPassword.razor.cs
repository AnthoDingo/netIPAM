namespace netIPAM.Components.Pages.Admin.Users
{
    public partial class ResetPassword : LocalizedComponentBase
    {
        [Inject]
        private UserService Users { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int Id { get; set; }
        [SupplyParameterFromForm] private FormModel _form { get; set; } = new();
        private User? _user;
        private string? _error;

        protected override async Task OnParametersSetAsync()
        {
            _user = await Users.FindByIdAsync(Id);
            if (_user is null) Nav.NavigateTo("/admin/users");
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(_form.Password))
            {
                _error = "Password required.";
                return;
            }
            await Users.ResetPasswordAsync(Id, _form.Password);
            Nav.NavigateTo("/admin/users");
        }

        public class FormModel { public string Password { get; set; } = string.Empty; }
    }
}
