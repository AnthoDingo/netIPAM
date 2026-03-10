namespace netIPAM.Components.Administration
{
    public partial class AdminSubMenuItem
    {
        [Inject]
        protected NavigationManager NavManager { get; set; } = default!;

        [Parameter]
        [EditorRequired]
        public string Name { get; set; }

        [Parameter]
        [EditorRequired]
        public string Url { get; set; }

        internal string Class { 
            get
            {
                return NavManager.Uri.TrimEnd('/').EndsWith(Url.TrimEnd('/'), StringComparison.OrdinalIgnoreCase) ? "active" : string.Empty;
            } 
        }

    }
}
