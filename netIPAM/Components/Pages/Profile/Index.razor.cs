using Microsoft.AspNetCore.Components.Authorization;
using netIPAM.Identity;

namespace netIPAM.Components.Pages.Profile
{
    public partial class Index
    {
        [Inject] private UserService Users { get; set; } = default!;
        [Inject] private IPasswordHasher Hasher { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthState { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        private User? _user;
        private string _activeTab = "info";
        private string _langCode = "en";
        private bool _saved;
        private string? _error;
        private bool _pwdSaved;
        private string? _pwdError;
        private PasswordForm _pwdForm = new();

        private static readonly (string code, string flag, string label)[] _languages =
        [
            ("en", "🇬🇧", "English"),
            ("fr", "🇫🇷", "Français"),
            ("it", "🇮🇹", "Italiano"),
            ("es", "🇪🇸", "Español"),
            ("de", "🇩🇪", "Deutsch"),
        ];

        protected override async Task OnInitializedAsync()
        {
            var auth = await AuthState.GetAuthenticationStateAsync();
            var username = auth.User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) { Nav.NavigateTo("/login"); return; }

            _user = await Users.FindByUsernameAsync(username);
            if (_user is null) { Nav.NavigateTo("/login"); return; }

            // Retrouver la langue stockée dans le champ Lang (index du tableau)
            _langCode = _user.Lang is int idx && idx >= 0 && idx < _languages.Length
                ? _languages[idx].code
                : "en";

            // Valeur par défaut pour le thème
            _user.Theme ??= "light";
        }

        private async Task SaveInfo()
        {
            await Save();
        }

        private async Task SaveDisplay()
        {
            // Persister le code langue comme index dans le tableau
            var idx = Array.FindIndex(_languages, l => l.code == _langCode);
            _user!.Lang = idx >= 0 ? idx : 0;
            await Save();
        }

        private async Task SaveNotifications()
        {
            await Save();
        }

        private async Task Save()
        {
            _error = null;
            _saved = false;
            try
            {
                await Users.UpdateAsync(_user!);
                _saved = true;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
        }

        private async Task SavePassword()
        {
            _pwdError = null;
            _pwdSaved = false;

            if (string.IsNullOrWhiteSpace(_pwdForm.Current))
            { _pwdError = "Please enter your current password."; return; }

            if (string.IsNullOrWhiteSpace(_pwdForm.New) || _pwdForm.New.Length < 8)
            { _pwdError = "New password must be at least 8 characters."; return; }

            if (_pwdForm.New != _pwdForm.Confirm)
            { _pwdError = "Passwords do not match."; return; }

            // Vérifier le mot de passe actuel
            if (string.IsNullOrEmpty(_user!.Password) || !Hasher.Verify(_pwdForm.Current, _user.Password))
            { _pwdError = "Current password is incorrect."; return; }

            try
            {
                await Users.ResetPasswordAsync(_user.Id, _pwdForm.New);
                _pwdForm = new();
                _pwdSaved = true;
            }
            catch (Exception ex)
            {
                _pwdError = ex.Message;
            }
        }

        public class PasswordForm
        {
            public string Current { get; set; } = string.Empty;
            public string New { get; set; } = string.Empty;
            public string Confirm { get; set; } = string.Empty;
        }
    }
}
