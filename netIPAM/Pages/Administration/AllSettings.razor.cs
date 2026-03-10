using netIPAM.Components;

namespace netIPAM.Pages.Administration
{
    public partial class AllSettings
    {
        private List<WidgetGroup> Groups = new();
        private List<WidgetGroup> FilteredGroups = new();

        protected override async Task OnInitializedAsync()
        {
            Groups.Add(new WidgetGroup()
            {
                Title = "Server management",
                Widgets = new List<WidgetDash>()
                {
                    new WidgetDash(){Title = "netIPAM settings", Description = "netIPAM server settings", Icon = "fa fa-cogs", Url = "/administration/settings/"},
                    new WidgetDash(){Title = "Users", Description = "User management", Icon = "fa fa-user", Url = "/administration/users/"},
                    new WidgetDash(){Title = "Groups", Description = "User group management", Icon = "fa fa-group", Url = "/administration/groups/"},
                    new WidgetDash(){Title = "Authentication methods", Description = "Manage user authentication methods and servers", Icon = "fa fa-server", Url = "/administration/authentication-methods/"},
                    new WidgetDash(){Title = "2FA", Description = "Two-factor authentication with TOTP provider", Icon = "fa fa-shield", Url = "/administration/2fa/"},
                    new WidgetDash(){Title = "Password policy", Description = "Set user password policy", Icon = "fa fa-unlock", Url = "/administration/password-policy/"},
                    new WidgetDash(){Title = "Mail settings", Description = "Set mail parameters and mail server settings", Icon = "fa fa-envelope-o", Url = "/administration/mail/"},
                    new WidgetDash(){Title = "API", Description = "API settings", Icon = "fa fa-cogs", Url = "/administration/api/"},
                    new WidgetDash(){Title = "Scan agents", Description = "netIPAM Scan agents", Icon = "fa fa-user-secret", Url = "/administration/scan-agents/"},
                    new WidgetDash(){Title = "Languages", Description = "Manage languages", Icon = "fa fa-language", Url = "/administration/languages/"},
                    new WidgetDash(){Title = "Widgets", Description = "Manage widget settings", Icon = "fa fa-tachometer", Url = "/administration/widgets/"},
                    new WidgetDash(){Title = "Tags", Description = "Manage tags", Icon = "fa fa-tag", Url = "/administration/tags/"},
                    new WidgetDash(){Title = "Edit instructions", Description = "Set netIPAM instructions for end users", Icon = "fa fa-info", Url = "/administration/instructions/"},
                }
            });

            Groups.Add(new WidgetGroup()
            {
                Title = "IP related management",
                Widgets = new List<WidgetDash>()
                {
                    new WidgetDash(){Title = "Customers", Description = "Customer management", Icon = "fa fa-group", Url = "/administration/customers/"},
                    new WidgetDash(){Title = "Sections", Description = "Section management", Icon = "fa fa-server", Url = "/administration/sections/"},
                    new WidgetDash(){Title = "VLAN", Description = "VLAN management", Icon = "fa fa-cloud", Url = "/administration/vlans/"},
                    new WidgetDash(){Title = "NAT", Description = "NAT settings", Icon = "fa fa-exchange", Url = "/administration/nat/"},
                    new WidgetDash(){Title = "Routing", Description = "Routing management", Icon = "fa fa-exchange", Url = "/administration/routing/"},
                    new WidgetDash(){Title = "Nameservers", Description = "Recursive nameserver sets for subnets", Icon = "fa fa-cloud", Url = "/administration/nameservers/"},
                    new WidgetDash(){Title = "Import / Export", Description = "Import/Export IP related data (VRF, VLAN, Subnets, IP, Devices)", Icon = "fa fa-upload", Url = "/administration/import-export/"},
                    new WidgetDash(){Title = "RIPE import", Description = "Import subnets from RIPE", Icon = "fa fa-cloud-download", Url = "/administration/ripe-import/"},
                    new WidgetDash(){Title = "Filter IP fields", Description = "Select which default address fields to display", Icon = "fa fa-filter", Url = "/administration/filter-fields/"},
                    new WidgetDash(){Title = "Required IP fields", Description = "Select which address fields are required to be filled when creating address.", Icon = "fa fa-filter", Url = "/administration/required-fields/"},
                    new WidgetDash(){Title = "Custom fields", Description = "Manage custom fields", Icon = "fa fa-magic", Url = "/administration/custom-fields/"},
                }
            });

            Groups.Add(new WidgetGroup()
            {
                Title = "Device management",
                Widgets = new List<WidgetDash>()
                {
                    new WidgetDash(){Title = "Devices", Description = "Device management", Icon = "fa fa-desktop", Url = "/administration/devices/"},
                    new WidgetDash(){Title = "Locations", Description = "Locations", Icon = "fa fa-map", Url = "/administration/locations/"},
                }
            });

            Groups.Add(new WidgetGroup()
            {
                Title = "Tools",
                Widgets = new List<WidgetDash>()
                {
                    new WidgetDash(){Title = "Version check", Description = "Check for latest version of netIPAM", Icon = "fa fa-check", Url = "/administration/version-check/"},
                    new WidgetDash(){Title = "Verify database", Description = "Verify that database files are installed ok", Icon = "fa fa-magic", Url = "/administration/verify-database/"},
                    new WidgetDash(){Title = "Replace fields", Description = "Search and replace content in database", Icon = "fa fa-search-plus", Url = "/administration/replace-fields/"},
                }
            });

            FilteredGroups = Groups;
        }

        public string Filter { get; set; } = string.Empty;

        private void OnFilterChanged()
        {
            FilteredGroups = Groups
                .Select(g => new WidgetGroup { 
                    Title = g.Title, 
                    Widgets = g.Widgets
                    .Where(w => w.Title.Contains(Filter, StringComparison.OrdinalIgnoreCase)).ToList()
                })
                .Where(g => g.Widgets.Any())
                .ToList();

            StateHasChanged();
        }


        private class WidgetGroup
        {
            public string Title { get; set; }
            public List<WidgetDash> Widgets { get; set; } = new();
        }
    }
}
