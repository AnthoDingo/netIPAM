using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            Nav.NavigateTo("/login", forceLoad: true);
        }
    }
}
