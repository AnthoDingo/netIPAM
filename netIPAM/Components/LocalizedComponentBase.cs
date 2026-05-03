using Microsoft.AspNetCore.Components;
using netIPAM.Services;

namespace netIPAM.Components;

/// <summary>
/// Classe de base pour tout composant Blazor qui affiche des textes traduits.
/// S'abonne à OnLanguageChanged et force un re-render automatique.
/// Tous les composants héritant de cette classe réagissent immédiatement
/// au changement de langue — aucune navigation ni rechargement nécessaire.
/// </summary>
public abstract class LocalizedComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject] protected LocalizationService Localization { get; set; } = default!;

    /// <summary>Raccourci : T("common.save") → "Save" / "Enregistrer" / "Salva"…</summary>
    protected string T(string key) => Localization.Get(key);

    protected override void OnInitialized()
    {
        Localization.OnLanguageChanged += HandleLanguageChanged;
    }

    private void HandleLanguageChanged()
    {
        // Tous les composants sont InteractiveServer — StateHasChanged est sûr en direct
        StateHasChanged();
    }

    public virtual ValueTask DisposeAsync()
    {
        Localization.OnLanguageChanged -= HandleLanguageChanged;
        return ValueTask.CompletedTask;
    }
}
