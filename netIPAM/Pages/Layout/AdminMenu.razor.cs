using netIPAM.Components.Administration;

namespace netIPAM.Pages.Layout
{
    public partial class AdminMenu
    {
        protected override async Task OnInitializedAsync()
        {
            
        }
        
        public class AdminMenuGroup
        {
            public string Name { get; set; }
            public List<AdminMenuItem> Items { get; set; }  = new List<AdminMenuItem>();
        }
    }
}
