namespace netIPAM;

/// <summary>
/// Singleton indiquant s'il existe des migrations EF Core en attente.
/// Alimenté au démarrage et mis à jour après application des migrations.
/// </summary>
public class MigrationState
{
    private readonly List<string> _pending = [];

    public bool HasPending        => _pending.Count > 0;
    public IReadOnlyList<string> Pending => _pending.AsReadOnly();

    public void SetPending(IEnumerable<string> migrations)
    {
        _pending.Clear();
        _pending.AddRange(migrations);
    }

    public void MarkApplied() => _pending.Clear();
}
