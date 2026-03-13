using netIPAM.Models;
using netIPAM.Services;

namespace netIPAM.Pages.Administration
{
    public partial class AllSettings
    {
        [Inject]
        private MenuItemsService MenuItems { get; set; } = default!;

        private List<AdminItem> Groups = new();
        private List<AdminItem> FilteredGroups = new();

        protected override async Task OnInitializedAsync()
        {
            Groups = MenuItems.AdminItems;

            FilteredGroups = Groups;
        }

        public string Filter { get; set; } = string.Empty;

        private void OnFilterChanged()
        {
            FilteredGroups = Groups
                .Select(g => new AdminItem { 
                    Title = g.Title, 
                    Icon = g.Icon,
                    Items = g.Items
                    .Where(w => w.Title.Contains(Filter, StringComparison.OrdinalIgnoreCase)).ToList()
                })
                .Where(g => g.Items.Any())
                .ToList();

            StateHasChanged();
        }
    }
}
