using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace netIPAM.Components
{
    public partial class WidgetDash
    {

        [Parameter]
        [EditorRequired]
        public string Icon { get; set; }

        [Parameter]
        [EditorRequired]
        public string Title { get; set; }

        [Parameter]
        [EditorRequired]
        public string Description { get; set; }

        [Parameter]
        [EditorRequired]
        public string Url { get; set; }
    }
}
