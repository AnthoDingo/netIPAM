using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace netIPAM.Components.Pages.Auth
{
    public partial class Logout
    {
        [Inject]
        private IHttpContextAccessor HttpCtx { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            HttpContext? ctx = HttpCtx.HttpContext;
            if (ctx is not null)
                await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Nav.NavigateTo("/login", forceLoad: true);
            HttpCtx.HttpContext!.Response.Redirect("/ogin");
        }
    }
}
