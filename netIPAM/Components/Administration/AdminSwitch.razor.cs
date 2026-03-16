using Microsoft.JSInterop;

namespace netIPAM.Components.Administration
{
    public partial class AdminSwitch
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Parameter]
        [Required]
        public required string Name { get; set; }

        [Parameter]
        [Required]
        public required string Title { get; set; }

        [Parameter]
        [EditorRequired]
        public bool Value { get; set; }

        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }

        //[Parameter]
        //public Expression<Func<bool>> ValueExpression { get; set; }

        [Parameter]
        [Required]
        public required string Description { get; set; }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            StateHasChanged();
        }

        private async Task OnStateChanged(bool newValue)
        {
            Value = newValue;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(newValue);
            }
        }

    }
}
