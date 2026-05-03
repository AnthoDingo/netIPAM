using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace netIPAM.Components.Pages.Auth
{
    public partial class Login
    {
        [Inject]
        private UserService Users { get; set; } = default!;

        [Inject]
        private IHttpContextAccessor HttpCtx { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [SupplyParameterFromForm] private LoginForm _form { get; set; } = new();
        [SupplyParameterFromQuery] public string? ReturnUrl { get; set; }
        private string? _error;

        private async Task HandleLogin()
        {
            User? user = await Users.AuthenticateLocalAsync(_form.Username, _form.Password);
            if (user is null) { _error = "Identifiants invalides."; return; }

            List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.Name,           user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email,          user.Email ?? string.Empty),
        };
            if (!string.IsNullOrEmpty(user.Role))
                claims.Add(new(ClaimTypes.Role, user.Role));

            ClaimsPrincipal principal = new(
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            await HttpCtx.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Server-side redirect for POST handlers
            HttpCtx.HttpContext!.Response.Redirect(ReturnUrl ?? "/");
            return;
        }

        public class LoginForm
        {
            [Required] public string Username { get; set; } = string.Empty;
            [Required] public string Password { get; set; } = string.Empty;
        }
    }
}
