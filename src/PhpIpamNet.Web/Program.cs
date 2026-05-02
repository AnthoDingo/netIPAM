using System.Security.Claims;
using Fido2NetLib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Infrastructure;
using PhpIpamNet.Infrastructure.Data;
using PhpIpamNet.Infrastructure.Services;
using PhpIpamNet.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server avec rendu interactif côté serveur
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Persistence (DbContext + services métier + Fido2)
builder.Services.AddPhpIpamPersistence(builder.Configuration);

// Authentification cookie
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath         = "/login";
        opt.LogoutPath        = "/logout";
        opt.AccessDeniedPath  = "/login";
        opt.ExpireTimeSpan    = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
        opt.Cookie.Name       = "phpipamnet.auth";
        opt.Cookie.HttpOnly   = true;
        opt.Cookie.SameSite   = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin",         p => p.RequireRole("Administrator"));
    opt.AddPolicy("Operator",      p => p.RequireRole("Administrator", "Operator"));
    opt.AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// ─────────────────────────────────────────────────────────────────────────────
// API WebAuthn (Passkey) — endpoints HTTP minimaux
// Doivent être en dehors du pipeline Blazor/antiforgery pour que le browser
// WebAuthn API puisse y accéder directement via fetch().
// ─────────────────────────────────────────────────────────────────────────────

var passkey = app.MapGroup("/api/passkey").DisableAntiforgery();

// 1. Options d'enregistrement (utilisateur connecté)
passkey.MapPost("/register/options", async (
    HttpContext ctx,
    PasskeyService pkService,
    UserService userService) =>
{
    if (ctx.User.Identity?.IsAuthenticated != true) return Results.Unauthorized();
    var user = await userService.FindByUsernameAsync(ctx.User.Identity.Name!);
    if (user is null) return Results.Unauthorized();

    var (options, sessionKey) = await pkService.GetRegistrationOptionsAsync(user);
    return Results.Ok(new { options, sessionKey });
}).RequireAuthorization();

// 2. Complétion de l'enregistrement
passkey.MapPost("/register/complete", async (
    HttpContext ctx,
    PasskeyRegistrationCompleteRequest req,
    PasskeyService pkService,
    UserService userService) =>
{
    if (ctx.User.Identity?.IsAuthenticated != true) return Results.Unauthorized();
    var user = await userService.FindByUsernameAsync(ctx.User.Identity.Name!);
    if (user is null) return Results.Unauthorized();

    try
    {
        var cred = await pkService.CompleteRegistrationAsync(
            user, req.AttestationResponse, req.SessionKey, req.DeviceName ?? "Passkey");
        return Results.Ok(new { success = true, credentialId = cred.Id, deviceName = cred.DeviceName });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
}).RequireAuthorization();

// 3. Options d'authentification (anonyme — début du flow login)
passkey.MapPost("/auth/options", (PasskeyService pkService) =>
{
    var (options, sessionKey) = pkService.GetAuthenticationOptions();
    return Results.Ok(new { options, sessionKey });
});

// 4. Complétion de l'authentification — pose le cookie de session
passkey.MapPost("/auth/complete", async (
    HttpContext ctx,
    PasskeyAuthCompleteRequest req,
    PasskeyService pkService) =>
{
    var user = await pkService.CompleteAuthenticationAsync(req.AssertionResponse, req.SessionKey);
    if (user is null)
        return Results.Json(new { success = false, error = "Authentification refusée." }, statusCode: 401);
    if (string.Equals(user.Disabled, "Yes", StringComparison.OrdinalIgnoreCase))
        return Results.Json(new { success = false, error = "Compte désactivé." }, statusCode: 403);

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name,           user.Username),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email,          user.Email ?? string.Empty),
    };
    if (!string.IsNullOrEmpty(user.Role))
        claims.Add(new Claim(ClaimTypes.Role, user.Role));

    await ctx.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

    return Results.Ok(new { success = true });
});

// ─────────────────────────────────────────────────────────────────────────────

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// Migrations + seed au démarrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PhpIpamDbContext>();
    await db.Database.MigrateAsync();
}
await app.Services.SeedAsync();

app.Run();

// DTOs pour les endpoints passkey (top-level records .NET 10)
public record PasskeyRegistrationCompleteRequest(
    AuthenticatorAttestationRawResponse AttestationResponse,
    string SessionKey,
    string? DeviceName);

public record PasskeyAuthCompleteRequest(
    AuthenticatorAssertionRawResponse AssertionResponse,
    string SessionKey);
