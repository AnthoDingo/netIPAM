using netIPAM.Models;

namespace netIPAM.Services
{
    internal class MenuItemsService
    {

        public List<ToolItem> ToolItems { get; private set; } 
        public List<AdminItem> AdminItems { get; private set; }

        public MenuItemsService()
        {
            InitToolsItems();
            InitAdminItems();
        }

        private void InitToolsItems()
        {
            ToolItems = new List<ToolItem>()
            {
                new ToolItem() { Name = "Customers", Icon = "fa-users", Tooltip = "Customers", Link = "/tools/customers/", ShowInMenuBar = true },
                new ToolItem() { Name = "VLAN", Icon = "fa-cloud", Tooltip = "Show VLANs and belonging subnets", Link = "/tools/vlan/", ShowInMenuBar = true },
                new ToolItem() { Name = "VRF", Icon = "fa-cloud", Tooltip = "Show VRFs and belonging networks", Link = "/tools/vrf/" },
                new ToolItem() { Name = "NAT", Icon = "fa-exchange", Tooltip = "Nat translations", Link = "/tools/nat/" },
                new ToolItem() { Name = "Locations", Icon = "fa-map", Tooltip = "Show locations", Link = "/tools/locations/" },
                new ToolItem() { Name = "Devices", Icon = "fa-desktop", Tooltip = "Show all configured devices", Link = "/tools/devices/" },
                new ToolItem() { Name = "Racks", Icon = "fa-bars", Tooltip = "Show racks", Link = "/tools/racks/" },
                new ToolItem() { Name = "Circuits", Icon = "fa-random", Tooltip = "Show circuits", Link = "/tools/circuits/" },
                new ToolItem() { Name = "Routing", Icon = "fa-exchange", Tooltip = "Show routing", Link = "/tools/routing/" },
                new ToolItem() { Name = "PSTN", Icon = "fa-phone", Tooltip = "PSTN prefixes", Link = "/tools/pstn-prefixes/" },
                new ToolItem() { Name = "Vaults", Icon = "fa-key", Tooltip = "Vaults", Link = "/tools/vaults/" },
                new ToolItem() { Name = "Search", Icon = "fa-search", Tooltip = "Search database Addresses, subnets and VLANs", Link = "/tools/search/" },
            };
        }

        private void InitAdminItems()
        {
            AdminItems = new List<AdminItem>()
            {
                new AdminItem()
                {
                    Title = "Server management",
                    Icon = "fa fa-cogs",
                    Items = new List<AdminSubItem>()
                    {
                        new AdminSubItem(){Title = "netIPAM settings", Description = "netIPAM server settings", Icon = "fa fa-cogs", Url = "/administration/settings/"},
                        new AdminSubItem(){Title = "Users", Description = "User management", Icon = "fa fa-user", Url = "/administration/users/"},
                        new AdminSubItem(){Title = "Groups", Description = "User group management", Icon = "fa fa-group", Url = "/administration/groups/"},
                        new AdminSubItem(){Title = "Authentication methods", Description = "Manage user authentication methods and servers", Icon = "fa fa-server", Url = "/administration/authentication-methods/"},
                        new AdminSubItem(){Title = "2FA", Description = "Two-factor authentication with TOTP provider", Icon = "fa fa-shield", Url = "/administration/2fa/"},
                        new AdminSubItem(){Title = "Password policy", Description = "Set user password policy", Icon = "fa fa-unlock", Url = "/administration/password-policy/"},
                        new AdminSubItem(){Title = "Mail settings", Description = "Set mail parameters and mail server settings", Icon = "fa fa-envelope-o", Url = "/administration/mail/"},
                        new AdminSubItem(){Title = "API", Description = "API settings", Icon = "fa fa-cogs", Url = "/administration/api/"},
                        new AdminSubItem(){Title = "Scan agents", Description = "netIPAM Scan agents", Icon = "fa fa-user-secret", Url = "/administration/scan-agents/"},
                        new AdminSubItem(){Title = "Languages", Description = "Manage languages", Icon = "fa fa-language", Url = "/administration/languages/"},
                        new AdminSubItem(){Title = "Widgets", Description = "Manage widget settings", Icon = "fa fa-tachometer", Url = "/administration/widgets/"},
                        new AdminSubItem(){Title = "Tags", Description = "Manage tags", Icon = "fa fa-tag", Url = "/administration/tags/"},
                        new AdminSubItem(){Title = "Edit instructions", Description = "Set netIPAM instructions for end users", Icon = "fa fa-info", Url = "/administration/instructions/"},
                    }
                },
                new AdminItem()
                {
                    Title = "IP related management",
                    Icon = "fa fa-sitemap",
                    Items = new List<AdminSubItem>()
                    {
                        new AdminSubItem(){Title = "Customers", Description = "Customer management", Icon = "fa fa-group", Url = "/administration/customers/"},
                        new AdminSubItem(){Title = "Sections", Description = "Section management", Icon = "fa fa-server", Url = "/administration/sections/"},
                        new AdminSubItem(){Title = "VLAN", Description = "VLAN management", Icon = "fa fa-cloud", Url = "/administration/vlans/"},
                        new AdminSubItem(){Title = "NAT", Description = "NAT settings", Icon = "fa fa-exchange", Url = "/administration/nat/"},
                        new AdminSubItem(){Title = "Routing", Description = "Routing management", Icon = "fa fa-exchange", Url = "/administration/routing/"},
                        new AdminSubItem(){Title = "Nameservers", Description = "Recursive nameserver sets for subnets", Icon = "fa fa-cloud", Url = "/administration/nameservers/"},
                        new AdminSubItem(){Title = "Import / Export", Description = "Import/Export IP related data (VRF, VLAN, Subnets, IP, Devices)", Icon = "fa fa-upload", Url = "/administration/import-export/"},
                        new AdminSubItem(){Title = "RIPE import", Description = "Import subnets from RIPE", Icon = "fa fa-cloud-download", Url = "/administration/ripe-import/"},
                        new AdminSubItem(){Title = "Filter IP fields", Description = "Select which default address fields to display", Icon = "fa fa-filter", Url = "/administration/filter-fields/"},
                        new AdminSubItem(){Title = "Required IP fields", Description = "Select which address fields are required to be filled when creating address.", Icon = "fa fa-filter", Url = "/administration/required-fields/"},
                        new AdminSubItem(){Title = "Custom fields", Description = "Manage custom fields", Icon = "fa fa-magic", Url = "/administration/custom-fields/"},
                    }
                },
                new AdminItem()
                {
                    Title = "Device management",
                    Icon = "fa fa-desktop",
                    Items = new List<AdminSubItem>()
                    {
                        new AdminSubItem(){Title = "Devices", Description = "Device management", Icon = "fa fa-desktop", Url = "/administration/devices/"},
                        new AdminSubItem(){Title = "Locations", Description = "Locations", Icon = "fa fa-map", Url = "/administration/locations/"},
                    }
                },
                new AdminItem()
                {
                    Title = "Tools",
                    Icon = "fa fa-wrench",
                    Items = new List<AdminSubItem>()
                    {
                        new AdminSubItem(){Title = "Version check", Description = "Check for latest version of netIPAM", Icon = "fa fa-check", Url = "/administration/version-check/"},
                        new AdminSubItem(){Title = "Verify database", Description = "Verify that database files are installed ok", Icon = "fa fa-magic", Url = "/administration/verify-database/"},
                        new AdminSubItem(){Title = "Replace fields", Description = "Search and replace content in database", Icon = "fa fa-search-plus", Url = "/administration/replace-fields/"},
                    }
                }
            };
            
        }
                
    }
}