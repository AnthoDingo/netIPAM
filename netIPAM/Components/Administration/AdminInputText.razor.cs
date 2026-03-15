using System.Linq.Expressions;

namespace netIPAM.Components.Administration
{
    public partial class AdminInputText
    {

        [Parameter]
        [EditorRequired]
        public string Name { get; set; }

        [Parameter]
        [EditorRequired]
        public string Title { get; set; }

        [Parameter]
        [EditorRequired]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<string>> ValueExpression { get; set; }

        [Parameter]
        [EditorRequired]
        public string Description { get; set; }
    }
}
