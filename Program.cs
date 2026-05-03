using Microsoft.AspNetCore.Authentication.Cookies;
using netIPAM;
using netIPAM.Data;
using netIPAM.Services;
using netIPAM.Components;

var builder = WebApplication.CreateBuilder(args);

// ── Détection du besoin de setup ─────────────────────────────────
var setupSvc   = new SetupService(builder.Environment);
var setupState = new SetupState();

if (setupSvc.IsSetupRequired(builder.Configuration))
    setupState.MarkRequired();

builder.Services.AddSingleton(setupState);
builder.Services.AddSingleton<SetupService>();

// ── Blazor ───────────────────────────────────────────────────────
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Persistence (InMemory si setup requis, réel sinon) ──────────
builder.Services.AddPersistence(builder.Configuration);

// ── Auth cookie ──────────────────────────────────────────────────
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath         = "/login";
        opt.LogoutPath        = "/logout";
        opt.AccessDeniedPath  = "/login";
        opt.ExpireTimeSpan    = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
        opt.Cookie.Name       = "netipam.auth";
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

// ── Build ─────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware setup : doit être avant Antiforgery et Blazor
app.UseMiddleware<SetupMiddleware>();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// ── Migrations + seed (uniquement si setup déjà terminé) ─────────
if (!setupState.SetupRequired)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await app.Services.SeedAsync();
}

app.Run();
