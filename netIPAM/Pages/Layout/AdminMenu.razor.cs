using netIPAM.Components.Administration;
using netIPAM.Models;
using netIPAM.Services;

namespace netIPAM.Pages.Layout
{
    public partial class AdminMenu
    {
        [Inject]
        private MenuItemsService MenuItems { get; set; } = default!;

        private List<AdminItem> Items = new List<AdminItem>();

        protected override async Task OnInitializedAsync()
        {
            Items= MenuItems.AdminItems;
        }
    }
}
