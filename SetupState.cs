namespace netIPAM;

/// <summary>
/// Singleton partagé entre le middleware et la page de setup.
/// Indique si le wizard d'installation doit être affiché.
/// </summary>
public class SetupState
{
    public bool SetupRequired { get; private set; }

    public void MarkRequired()    => SetupRequired = true;
    public void MarkCompleted()   => SetupRequired = false;
}
