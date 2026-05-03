namespace netIPAM;

/// <summary>
/// Middleware qui intercepte toutes les requêtes HTTP quand le setup n'est pas terminé
/// et redirige vers /setup — sauf pour les ressources statiques et les endpoints Blazor.
/// </summary>
public class SetupMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SetupState      _state;

    // Préfixes qui ne doivent jamais être redirigés
    private static readonly string[] _bypass =
    [
        "/setup",
        "/_blazor",
        "/_framework",
        "/css/",
        "/js/",
        "/favicon"
    ];

    public SetupMiddleware(RequestDelegate next, SetupState state)
    {
        _next  = next;
        _state = state;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (_state.SetupRequired)
        {
            string? path = ctx.Request.Path.Value ?? "/";
            bool bypass = _bypass.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            if (!bypass && path != "/")
            {
                ctx.Response.Redirect("/setup");
                return;
            }

            // La racine / redirige aussi vers /setup quand setup requis
            if (path == "/" || path == "")
            {
                ctx.Response.Redirect("/setup");
                return;
            }
        }

        await _next(ctx);
    }
}
