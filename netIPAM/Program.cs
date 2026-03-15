using Microsoft.AspNetCore.Components.Authorization;
using netIPAM;
using netIPAM.DBContexts;
using netIPAM.Pages;
using netIPAM.Services;
//using netIPAM.Services.Account.Identity.IdentityRevalidatingAuthenticationStateProvider;


namespace netIPAM
{
    public class Program
    {
        private static WebApplicationBuilder builder;

        public static void Main(string[] args)
        {
            builder = WebApplication.CreateBuilder(args);

            IServiceCollection services = builder.Services;

            // Add services to the container.
            services.AddRazorComponents()
                    .AddInteractiveServerComponents();

            services.AddCascadingAuthenticationState();
            services.AddScoped<IdentityRedirectManager>();
            services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            services.ApplyAuthProviders(builder.Configuration);

            //string connectionString = builder.Configuration.GetConnectionString("mssql") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<AppDbContext>();
            //services.AddDbContextFactory<AppDbContext>();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentityCore<AppUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";

                // Tu peux aussi prévoir le coup pour les autres routes standards :
                // options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/access-denied";
            });

            services.AddSingleton<IEmailSender<AppUser>, IdentityNoOpEmailSender>();

            services.AddSingleton<MenuItemsService>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();


            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
