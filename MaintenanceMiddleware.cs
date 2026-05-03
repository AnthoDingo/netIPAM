namespace netIPAM;

/// <summary>
/// Redirige vers /maintenance si le mode maintenance est actif,
/// sauf pour les Administrators (qui peuvent toujours accéder au site).
/// Doit être placé APRÈS UseAuthentication() dans le pipeline.
/// </summary>
public class MaintenanceMiddleware
{
    private readonly RequestDelegate  _next;
    private readonly MaintenanceState _state;

    private static readonly string[] _bypass =
    [
        "/maintenance",
        "/login",
        "/logout",
        "/_blazor",
        "/_framework",
        "/css/",
        "/js/",
        "/favicon",
        "/setup",
        "/migrate"
    ];

    public MaintenanceMiddleware(RequestDelegate next, MaintenanceState state)
    {
        _next  = next;
        _state = state;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (_state.IsEnabled)
        {
            // Les administrateurs passent toujours (UseAuthentication a déjà tourné)
            bool isAdmin = ctx.User.IsInRole("Administrator");

            if (!isAdmin)
            {
                string? path = ctx.Request.Path.Value ?? "/";
                bool bypass = _bypass.Any(p =>
                    path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

                if (!bypass)
                {
                    ctx.Response.Redirect("/maintenance");
                    return;
                }
            }
        }

        await _next(ctx);
    }
}
