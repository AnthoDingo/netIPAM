using Microsoft.JSInterop;

namespace netIPAM.Components.Switch
{
    public partial class Switch : IAsyncDisposable
    {
        [Inject] 
        private IJSRuntime JS { get; set; }

        [Inject] 
        private ILogger<Switch> Logger { get; set; }

        #region Component Parameters

        /// <summary>
        /// Set UI when Switch State is On
        /// </summary>
        [Parameter]
        public string OnUiLabel { get; set; } = "ON";

        /// <summary>
        /// Set UI when Switch State is Off
        /// </summary>
        [Parameter]
        public string OffUiLabel { get; set; } = "OFF";

        /// <summary>
        /// Set CSS class names when Switch State is On
        /// </summary>
        [Parameter]
        public string OnUiStyle { get; set; } = "default";

        /// <summary>
        /// Set CSS class names when Switch State is Off
        /// </summary>
        [Parameter]
        public string OffUiStyle { get; set; } = "default";

        /// <summary>
        /// (Optional) Set Switch size in CSS string representation
        /// </summary>
        [Parameter]
        public string SwitchSize { get; set; } = "mini";

        /// <summary>
        /// (Optional) Set Switch CSS style
        /// </summary>
        [Parameter]
        public string SwitchStyle { get; set; } = null;

        /// <summary>
        /// (Optional) Set Switch width
        /// </summary>
        [Parameter]
        public int? SwitchWidth { get; set; }

        /// <summary>
        /// (Optional) Set Switch height
        /// </summary>
        [Parameter]
        public int? SwitchHeight { get; set; }

        /// <summary>
        /// Set Switch initial state (default is Off)
        /// </summary>
        [Parameter]
        public bool InitialState { get; set; } = true;

        private bool _currentState;

        /// <summary>
        /// Switch current state
        /// </summary>
        [Parameter]
        public bool State
        {
            get => _currentState;
            set
            {
                if (_currentState == value)
                {
                    return;
                }

                _currentState = value;
                SetSwitchButtonState(_currentState);
            }
        }

        /// <summary>
        /// Event handler when Switch state changed
        /// </summary>
        [Parameter]
        public EventCallback<bool> StateChanged { get; set; }

        #endregion

        #region Event Interop references

        private IJSObjectReference _switchButtonJsModule;
        private IJSObjectReference _checkBoxInputJsRef;
        private IJSObjectReference _switchBtnEventInvokeRef;

        private ElementReference _switchButtonContainer;
        
        private DotNetObjectReference<Switch> _dotNetInvokeRef;

        #endregion

        private readonly string _disposeTimeoutLogTemplate =
            $"Disposing JSInterop object {nameof(_switchButtonJsModule)} in {nameof(Switch)} component timeout";

        private const string SetStatusJsCall = @"setSwitchButtonStatus";

        private bool _pendingStateUpdate = false;
        private bool _hasPendingUpdate = false;


        /// <inheritdoc />
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
                return;

            var switchOption = new SwitchOption
            {
                Onlabel = OnUiLabel,
                Offlabel = OffUiLabel,
                Onstyle = OnUiStyle,
                Offstyle = OffUiStyle,
                Width = SwitchWidth,
                Height = SwitchHeight
            };

            if (!string.IsNullOrEmpty(SwitchSize))
            {
                switchOption.Size = SwitchSize;
            }

            if (!string.IsNullOrEmpty(SwitchStyle))
            {
                switchOption.Style = SwitchStyle;
            }

            _switchButtonJsModule = await JS.InvokeAsync<IJSObjectReference>("import", "/Components/Switch/Switch.razor.js");
            _checkBoxInputJsRef =
                await _switchButtonJsModule.InvokeAsync<IJSObjectReference>("createSwitchButton", _switchButtonContainer,
                    switchOption);

            //await _switchButtonJsModule.InvokeVoidAsync(SetStatusJsCall, _checkBoxInputJsRef, InitialState ? "on" : "off", true);
            await _switchButtonJsModule.InvokeVoidAsync(SetStatusJsCall, _checkBoxInputJsRef, _currentState ? "on" : "off", true);

            _switchBtnEventInvokeRef = await _switchButtonJsModule.InvokeAsync<IJSObjectReference>("createDotNetInvokeRef");

            _dotNetInvokeRef = DotNetObjectReference.Create(this);
            await _switchBtnEventInvokeRef.InvokeVoidAsync("init", _dotNetInvokeRef, _checkBoxInputJsRef);

            if (_hasPendingUpdate)
            {
                await _switchButtonJsModule.InvokeVoidAsync(
                    SetStatusJsCall, _checkBoxInputJsRef, _pendingStateUpdate ? "on" : "off", true);
                _hasPendingUpdate = false;
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool previousState = _currentState;

            await base.SetParametersAsync(parameters);

            if (_currentState != previousState)
            {
                if (_switchButtonJsModule != null)
                {
                    await _switchButtonJsModule.InvokeVoidAsync(
                        SetStatusJsCall, _checkBoxInputJsRef, _currentState ? "on" : "off", true);
                }
                else
                {
                    // JS pas encore prêt, on mémorise pour l'appliquer après
                    _pendingStateUpdate = _currentState;
                    _hasPendingUpdate = true;
                }
            }
        }

        /// <summary>
        /// Js interop method
        /// </summary>
        /// <param name="e"></param>
        [JSInvokable("SwitchBtnEventHandler")]
        public void OnSwitchButtonClicked(ChangeEventArgs e)
        {
            var status = e.Value?.ToString();
            var isChecked = bool.Parse(status ?? string.Empty);
            State = isChecked;
            if (StateChanged.HasDelegate)
            {
                StateChanged.InvokeAsync(isChecked);
            }
        }

        private async void SetSwitchButtonState(bool state)
        {
            var inputState = "off";
            if (state)
            {
                inputState = "on";
            }

            if (_switchButtonJsModule != null)
            {
                await _switchButtonJsModule.InvokeVoidAsync(SetStatusJsCall, _checkBoxInputJsRef, inputState, true);
            }
            StateHasChanged();
        }

        #region Dispose Pattern implementation

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Actual dispose object implementation
        /// </summary>
        /// <returns></returns>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dotNetInvokeRef?.Dispose();
                switch (_switchButtonJsModule)
                {
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                    case IAsyncDisposable asyncDisposable:
                        {
                            try
                            {
                                asyncDisposable.DisposeAsync().AsTask().RunSynchronously();
                            }
                            catch (OperationCanceledException ex)
                            {
                                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                                Logger.LogDebug(ex, _disposeTimeoutLogTemplate);
                            }

                            break;
                        }
                }
            }

            _dotNetInvokeRef = null;
            _switchButtonJsModule = null;
        }

        /// <summary>
        /// Actual dispose object implementation,
        /// see: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#implement-the-async-dispose-pattern
        /// </summary>
        /// <returns></returns>
        protected async virtual ValueTask DisposeAsyncCore()
        {
            _dotNetInvokeRef?.Dispose();
            _dotNetInvokeRef = null;

            if (_switchButtonJsModule != null)
            {
                try
                {
                    await _switchButtonJsModule.DisposeAsync().ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                    Logger.LogDebug(ex, _disposeTimeoutLogTemplate);
                }
                catch (JSDisconnectedException ex)
                {
                    Logger.LogDebug(ex, "JS interop disposal failed due to circuit disconnection");
                }
            }

            _switchButtonJsModule = null;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
