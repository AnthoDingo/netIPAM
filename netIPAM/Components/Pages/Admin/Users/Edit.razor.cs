namespace netIPAM.Components.Pages.Admin.Users
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private UserService Users { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }

        [SupplyParameterFromForm] private FormModel _form { get; set; } = new();

        private User? _user;
        private string? _error;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            if (_isNew)
                _user = new User { Disabled = "No", AuthMethod = 1 };
            else
            {
                _user = await Users.FindByIdAsync(Id!.Value);
                if (_user is null) Nav.NavigateTo("/admin/users");
            }
        }

        private async Task Save()
        {
            if (_user is null) return;
            _error = null;

            try
            {
                if (_isNew)
                {
                    if (string.IsNullOrWhiteSpace(_form.Password))
                    {
                        _error = "Password is required.";
                        return;
                    }
                    await Users.CreateAsync(_user, _form.Password);
                }
                else
                {
                    await Users.UpdateAsync(_user);
                }
                Nav.NavigateTo("/admin/users");
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
        }

        public class FormModel { public string Password { get; set; } = string.Empty; }
    }
}
