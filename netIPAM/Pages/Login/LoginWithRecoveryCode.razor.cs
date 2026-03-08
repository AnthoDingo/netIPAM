namespace netIPAM.Pages.Login
{
    public partial class LoginWithRecoveryCode
    {
        [Inject]
        protected SignInManager<AppUser> SignInManager { get; set; } = default!;

        [Inject]
        protected UserManager<AppUser> UserManager { get; set; } = default!;

        [Inject]
        protected IdentityRedirectManager RedirectManager { get; set; } = default!;

        [Inject]
        protected ILogger<LoginWithRecoveryCode> Logger { get; set; } = default!;

        private string? message;
        private AppUser user = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();

            // Ensure the user has gone through the username & password screen first
            user = await SignInManager.GetTwoFactorAuthenticationUserAsync() ??
                throw new InvalidOperationException("Unable to load two-factor authentication user.");
        }

        private async Task OnValidSubmitAsync()
        {
            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            var userId = await UserManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                Logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", userId);
                RedirectManager.RedirectTo(ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                Logger.LogWarning("User account locked out.");
                RedirectManager.RedirectTo("Account/Lockout");
            }
            else
            {
                Logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", userId);
                message = "Error: Invalid recovery code entered.";
            }
        }

        private sealed class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; } = "";
        }
    }
}
