using netIPAM.Models;
using netIPAM.Services;

namespace netIPAM.Pages.Layout
{
    public partial class NavMenu
    {
        [Inject]
        private MenuItemsService MenuItems { get; set; } = default!;

        private List<ToolItem> MenuToolItems;
        private List<ToolItem> ToolItems;
        private List<AdminItem> AdminItems;
        
        protected override async Task OnInitializedAsync()
        {
            MenuToolItems = MenuItems.ToolItems.Where(w => w.ShowInMenuBar).ToList();
            ToolItems = MenuItems.ToolItems;
            AdminItems = MenuItems.AdminItems;
        }

    }
}
