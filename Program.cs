using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using netIPAM;
using netIPAM.Data;
using netIPAM.Services;
using netIPAM.Components;

var builder = WebApplication.CreateBuilder(args);

// ── Détection setup requis ────────────────────────────────────────
var setupSvc   = new SetupService(builder.Environment);
var setupState = new SetupState();

if (setupSvc.IsSetupRequired(builder.Configuration))
    setupState.MarkRequired();

builder.Services.AddSingleton(setupState);
builder.Services.AddSingleton<SetupService>();

// ── MigrationState (singleton, alimenté après build) ─────────────
var migrationState = new MigrationState();
builder.Services.AddSingleton(migrationState);

// ── MaintenanceState (singleton, alimenté depuis DB après build) ──
var maintenanceState = new MaintenanceState();
builder.Services.AddSingleton(maintenanceState);

// ── Blazor ───────────────────────────────────────────────────────
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Persistence ──────────────────────────────────────────────────
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

// Middlewares setup et migrate — dans l'ordre
app.UseMiddleware<SetupMiddleware>();
app.UseMiddleware<MigrateMiddleware>();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// Maintenance : après auth pour pouvoir lire ctx.User.IsInRole
app.UseMiddleware<MaintenanceMiddleware>();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// ── Initialisation post-build ─────────────────────────────────────
if (!setupState.SetupRequired)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();

        if (pending.Count > 0)
        {
            // Des migrations sont en attente → le middleware redirigera vers /migrate
            migrationState.SetPending(pending);
            app.Logger.LogWarning(
                "{Count} migration(s) en attente : {Names}",
                pending.Count,
                string.Join(", ", pending));
        }
        else
        {
            // Base à jour → seed si nécessaire
            await app.Services.SeedAsync();

            // Charger l'état de maintenance depuis la DB
            var settings = await db.Settings.FirstOrDefaultAsync();
            if (settings?.MaintenanceMode == true)
            {
                maintenanceState.Enable();
                app.Logger.LogWarning("Mode maintenance actif au démarrage.");
            }
        }
    }
    catch (Exception ex)
    {
        // DB inaccessible (ex. premier démarrage avant setup)
        app.Logger.LogError(ex, "Impossible de vérifier les migrations");
    }
}

app.Run();
