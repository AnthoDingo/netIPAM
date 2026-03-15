using System.Linq.Expressions;

namespace netIPAM.Components.Administration
{
    public partial class AdminInputNumber
    {

        [Parameter]
        [EditorRequired]
        public string Name { get; set; }

        [Parameter]
        [EditorRequired]
        public string Title { get; set; }

        [Parameter]
        [EditorRequired]
        public int Value { get; set; }

        [Parameter]
        public EventCallback<int> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<int>> ValueExpression { get; set; }

        [Parameter]
        [EditorRequired]
        public string Description { get; set; }

        [Parameter]
        public int Min { get; set; } = 0;

        [Parameter]
        public int Max { get; set; } = 1024;
    }
}
