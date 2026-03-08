using Microsoft.AspNetCore.Components.Forms;
using System.Text.Encodings.Web;

namespace netIPAM.Pages.Login
{
    public partial class Register
    {
        [Inject]
        protected UserManager<AppUser> UserManager { get; set; } = default!;

        [Inject]
        protected IUserStore<AppUser> UserStore { get; set; } = default!;

        [Inject]
        protected SignInManager<AppUser> SignInManager { get; set; } = default!;

        [Inject]
        protected IEmailSender<AppUser> EmailSender { get; set; } = default!;

        [Inject]
        protected ILogger<Register> Logger { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IdentityRedirectManager RedirectManager { get; set; } = default!;

        private IEnumerable<IdentityError>? identityErrors;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

        protected override void OnInitialized()
        {
            Input ??= new();
        }

        public async Task RegisterUser(EditContext editContext)
        {
            AppUser user = CreateUser();

            await UserStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            IUserEmailStore<AppUser> emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            IdentityResult result = await UserManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                identityErrors = result.Errors;
                return;
            }

            Logger.LogInformation("User created a new account with password.");

            string userId = await UserManager.GetUserIdAsync(user);
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            string callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });

            await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

            if (UserManager.Options.SignIn.RequireConfirmedAccount)
            {
                RedirectManager.RedirectTo(
                    "Account/RegisterConfirmation",
                    new() { ["email"] = Input.Email, ["returnUrl"] = ReturnUrl });
            }
            else
            {
                await SignInManager.SignInAsync(user, isPersistent: false);
                RedirectManager.RedirectTo(ReturnUrl);
            }
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!UserManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)UserStore;
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = "";

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = "";
        }

    }
}
