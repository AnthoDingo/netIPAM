using Microsoft.AspNetCore.Authentication;

namespace netIPAM.Components.Login
{
    public partial class ExternalLoginPicker
    {
        [Inject]
        protected SignInManager<AppUser> SignInManager { get; set; } = default!;

        [Inject]
        protected IdentityRedirectManager RedirectManager { get; set; } = default!;

        private AuthenticationScheme[] externalLogins = [];

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            externalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
        }
    }
}
