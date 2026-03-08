using Microsoft.AspNetCore.Components;

namespace netIPAM.Pages.Shared
{
    public partial class RedirectToLogin
    {

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        protected override void OnInitialized()
        {
            NavigationManager.NavigateTo($"login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}", forceLoad: true);
        }
    }
}
