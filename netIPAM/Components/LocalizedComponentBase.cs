using Microsoft.AspNetCore.Components;
using netIPAM.Services;

namespace netIPAM.Components;

/// <summary>
/// Classe de base pour tout composant qui affiche des textes traduits.
/// S'abonne au OnLanguageChanged du LocalizationService et déclenche
/// StateHasChanged automatiquement — les composants fils n'ont rien à faire.
/// Expose T(key) comme raccourci pour Localization.Get(key).
/// </summary>
public abstract class LocalizedComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject] protected LocalizationService Localization { get; set; } = default!;

    /// <summary>Raccourci : T("common.save") → "Save" / "Enregistrer" / "Salva"…</summary>
    protected string T(string key) => Localization.Get(key);

    protected override void OnInitialized()
    {
        Localization.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged() => InvokeAsync(StateHasChanged);

    public virtual ValueTask DisposeAsync()
    {
        Localization.OnLanguageChanged -= OnLanguageChanged;
        return ValueTask.CompletedTask;
    }
}
