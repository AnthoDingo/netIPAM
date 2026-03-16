using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using netIPAM.Services;

namespace netIPAM.Pages.Login
{
    public partial class Login
    {

        [Inject]
        protected UserManager<AppUser> UserManager { get; set; } = default!;

        [Inject]
        protected SignInManager<AppUser> SignInManager { get; set; } = default!;

        [Inject]
        protected ILogger<Login> Logger { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IdentityRedirectManager RedirectManager { get; set; } = default!;

        [Inject]
        private CacheService CacheService { get; set; } = default!;

        private string? errorMessage;
        private string? successMessage;

        private bool isPasskeyEnabled = false;
        private string? errorPasskey;
        
        private EditContext editContext = default!;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm(FormName = "login")]
        private InputModel Input { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();

            editContext = new EditContext(Input);

            if (HttpMethods.IsGet(HttpContext.Request.Method))
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            bool value = await CacheService.GetSettingBoolAsync("passkeys");
            isPasskeyEnabled = value;
        }

        public async Task LoginUser()
        {
            errorMessage = null;
            successMessage = null;
            errorPasskey = null;

            if (string.IsNullOrEmpty(Input.Username) || string.IsNullOrEmpty(Input.Password))
            {
                errorMessage = "Please enter your username and password";
                return;
            }

            if (!string.IsNullOrEmpty(Input.Passkey?.Error))  
            {
                // errorMessage = $"Error: {Input.Passkey.Error}";
                errorPasskey = "Passkey authentication failed!";
                return;
            }

            SignInResult result;
            if (!string.IsNullOrEmpty(Input.Passkey?.CredentialJson))
            {
                // When performing passkey sign-in, don't perform form validation.
                result = await SignInManager.PasskeySignInAsync(Input.Passkey.CredentialJson);
            }
            else
            {
                // If doing a password sign-in, validate the form.
                if (!editContext.Validate())
                {
                    return;
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                result = await SignInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            }

            if (result.Succeeded)
            {
                Logger.LogInformation("User logged in.");
                RedirectManager.RedirectTo(ReturnUrl);
            }
            else if (result.RequiresTwoFactor)
            {
                RedirectManager.RedirectTo(
                    "login/2fa",
                    new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
            }
            else if (result.IsLockedOut)
            {
                Logger.LogWarning("User account locked out.");
                RedirectManager.RedirectTo("lockout");
            }
            else
            {
                errorMessage = "Error: Invalid login attempt.";
            }
        }

        private sealed class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            public string Username { get; set; } = "";

            //[Required]
            //[EmailAddress]
            //public string Email { get; set; } = "";

            //[Required]
            //[DataType(DataType.Text)]
            //public string Username { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            public Models.Account.PasskeyInputModel? Passkey { get; set; }
        }
    }
}
