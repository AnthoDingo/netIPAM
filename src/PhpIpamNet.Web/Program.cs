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

// Persistence (DbContext + services métier)
builder.Services.AddPhpIpamPersistence(builder.Configuration);

// Authentification cookie (équivalent ASP.NET Core des sessions PHP)
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

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// Migrations + seed au démarrage. À retirer pour les déploiements production stricts.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PhpIpamDbContext>();
    await db.Database.MigrateAsync();
}
await app.Services.SeedAsync();

app.Run();
