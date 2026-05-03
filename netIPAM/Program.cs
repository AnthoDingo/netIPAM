using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using netIPAM;
using netIPAM.Components;
using netIPAM.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Détection setup requis ────────────────────────────────────────
SetupService setupSvc = new(builder.Environment);
SetupState setupState = new();

if (setupSvc.IsSetupRequired(builder.Configuration))
    setupState.MarkRequired();

builder.Services.AddSingleton(setupState);
builder.Services.AddSingleton<SetupService>();

// ── MigrationState ────────────────────────────────────────────────
MigrationState migrationState = new();
builder.Services.AddSingleton(migrationState);

// ── MaintenanceState ──────────────────────────────────────────────
MaintenanceState maintenanceState = new();
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
        opt.LoginPath = "/login";
        opt.LogoutPath = "/logout";
        opt.AccessDeniedPath = "/login";
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
        opt.Cookie.Name = "netipam.auth";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", p => p.RequireRole("Administrator"));
    opt.AddPolicy("Operator", p => p.RequireRole("Administrator", "Operator"));
    opt.AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

// ── Build ─────────────────────────────────────────────────────────
WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<SetupMiddleware>();
app.UseMiddleware<MigrateMiddleware>();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<MaintenanceMiddleware>();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// ── Initialisation post-build ─────────────────────────────────────
if (!setupState.SetupRequired)
{
    using IServiceScope scope = app.Services.CreateScope();
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        List<string> pending = (await db.Database.GetPendingMigrationsAsync()).ToList();

        if (pending.Count > 0)
        {
            migrationState.SetPending(pending);
            app.Logger.LogWarning(
                "{Count} migration(s) en attente : {Names}",
                pending.Count,
                string.Join(", ", pending));
        }
        else
        {
            await app.Services.SeedAsync();

            Setting? settings = await db.Settings.FirstOrDefaultAsync();
            if (settings?.MaintenanceMode == true)
            {
                maintenanceState.Enable();
                app.Logger.LogWarning("Mode maintenance actif au démarrage.");
            }
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Impossible de vérifier les migrations");
    }
}

app.Run();
