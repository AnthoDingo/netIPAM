using System.Text.Encodings.Web;

namespace netIPAM.Pages.Login
{
    public partial class ResendEmailConfirmation
    {
        [Inject]
        protected UserManager<AppUser> UserManager { get; set; } = default!;

        [Inject]
        protected IEmailSender<AppUser> EmailSender { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IdentityRedirectManager RedirectManager { get; set; } = default!;

        private string? message;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        protected override void OnInitialized()
        {
            Input ??= new();
        }

        private async Task OnValidSubmitAsync()
        {
            var user = await UserManager.FindByEmailAsync(Input.Email!);
            if (user is null)
            {
                message = "Verification email sent. Please check your email.";
                return;
            }

            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
            await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

            message = "Verification email sent. Please check your email.";
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";
        }
    }
}
