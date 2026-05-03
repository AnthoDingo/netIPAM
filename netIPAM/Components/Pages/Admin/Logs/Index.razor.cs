namespace netIPAM.Components.Pages.Admin.Logs
{
    public partial class Index
    {
        [Inject]
        private LogService Logs { get; set; } = default!;

        private List<Log>? _logs;
        private int _total;
        private string _severity = "";

        protected override async Task OnInitializedAsync() => await Reload();

        private async Task Reload()
        {
            int? s = int.TryParse(_severity, out var v) ? v : null;
            _logs = await Logs.ListAsync(severityFilter: s);
            _total = await Logs.CountAsync();
        }

        private static MarkupString SeverityBadge(int? sev) => sev switch
        {
            0 => new("<span class=\"badge bg-secondary\">Info</span>"),
            1 => new("<span class=\"badge bg-warning text-dark\">Warning</span>"),
            2 => new("<span class=\"badge bg-danger\">Error</span>"),
            _ => new("—")
        };
    }
}
