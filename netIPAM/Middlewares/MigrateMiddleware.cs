namespace netIPAM;

/// <summary>
/// Redirige toutes les requêtes vers /migrate quand des migrations EF Core
/// sont en attente — sauf les ressources statiques, les endpoints Blazor,
/// le login et la page /migrate elle-même.
/// </summary>
public class MigrateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MigrationState _state;

    private static readonly string[] _bypass =
    [
        "/migrate",
        "/login",
        "/logout",
        "/_blazor",
        "/_framework",
        "/css/",
        "/js/",
        "/favicon"
    ];

    public MigrateMiddleware(RequestDelegate next, MigrationState state)
    {
        _next = next;
        _state = state;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (_state.HasPending)
        {
            string? path = ctx.Request.Path.Value ?? "/";
            bool bypass = _bypass.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            if (!bypass)
            {
                ctx.Response.Redirect("/migrate");
                return;
            }
        }

        await _next(ctx);
    }
}
