namespace netIPAM;

/// <summary>
/// Singleton indiquant si le mode maintenance est actif.
/// Chargé depuis la table settings au démarrage,
/// mis à jour en mémoire quand l'admin bascule le toggle.
/// </summary>
public class MaintenanceState
{
    public bool IsEnabled { get; private set; }

    public void Enable()           => IsEnabled = true;
    public void Disable()          => IsEnabled = false;
    public void Set(bool value)    => IsEnabled = value;
}
