using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using netIPAM.DBContexts;
using netIPAM.Models;
using netIPAM.Services;

namespace netIPAM.Pages.Administration
{
    public partial class Settings
    {

        [Inject]
        private IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;

        [Inject]
        private CacheService CacheService { get; set; } = default!;


        [SupplyParameterFromForm(FormName = "settings")]
        private InputModel Input { get; set; } = default!;

        private EditContext editContext = default!;
        private Modal loadingModal = default!;

        private Dictionary<string, string> _settings = new();
        private Dictionary<string, string> _originalSettings = new();

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            editContext = new EditContext(Input);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //await base.OnAfterRenderAsync(firstRender);

            if(!firstRender)
                return;

            //await Task.Delay(300);
            //await loadingModal.ShowAsync();

            using AppDbContext db = DbFactory.CreateDbContext();
            //settings = await db.Settings
            //    .ToDictionaryAsync(s => s.Name, s => s.Value ?? "");
            _settings = await CacheService.GetSettingsAsync();
            _originalSettings = new Dictionary<string, string>(_settings);

            #region Site settings

            Input.SiteTitle = CacheService.GetString(_settings, "siteTitle");
            Input.SiteDomain = CacheService.GetString(_settings, "siteDomain");
            Input.SiteUrl = CacheService.GetString(_settings, "siteURL");
            Input.LoginText = CacheService.GetString(_settings, "siteLoginText");
            Input.PermissionPropagation = CacheService.GetBool(_settings, "permissionPropagate");
            Input.MaxVLAN = CacheService.GetInt(_settings, "vlanMax");
            Input.MaintenanceMode = CacheService.GetBool(_settings, "maintaneanceMode");

            #endregion

            #region Admin settings

            Input.AdminName = CacheService.GetString(_settings, "siteAdminName");
            Input.AdminEmail = CacheService.GetString(_settings, "siteAdminMail");

            #endregion

            #region Feature settings

            Input.API = CacheService.GetBool(_settings, "api");
            Input.IPRequestModule = CacheService.GetBool(_settings, "enableIPrequests");
            Input.VRFSupport = CacheService.GetBool(_settings, "enableVRF");
            Input.NAT = CacheService.GetBool(_settings, "enableNAT");
            Input.PowerDNS = CacheService.GetBool(_settings, "enablePowerDNS");
            Input.DHCP = CacheService.GetBool(_settings, "enableDHCP");
            Input.FirewallZones = CacheService.GetBool(_settings, "enableFirewallZones");
            Input.ResolveDNS = CacheService.GetBool(_settings, "enableDNSresolving");
            Input.TemporaryShares = CacheService.GetBool(_settings, "tempShare");
            Input.Changelog = CacheService.GetBool(_settings, "enableChangelog");
            Input.MulticastModule = CacheService.GetBool(_settings, "enableMulticast");
            Input.ThresholdModule = CacheService.GetBool(_settings, "enableThreshold");
            Input.RackModule = CacheService.GetBool(_settings, "enableRACK");
            Input.CircuitsModule = CacheService.GetBool(_settings, "enableCircuits");
            Input.LocationsModule = CacheService.GetBool(_settings, "enableLocations");
            Input.SNMPModule = CacheService.GetBool(_settings, "enableSNMP");
            Input.PSTNModule = CacheService.GetBool(_settings, "enablePSNT");
            Input.CustomersModule = CacheService.GetBool(_settings, "enableCustomers");
            Input.RoutingModule = CacheService.GetBool(_settings, "enableRouting");
            Input.UpdateTags = CacheService.GetBool(_settings, "updateTags");
            Input.RequireUniqueSubnets = CacheService.GetBool(_settings, "enforceUnique");
            Input.AllowDuplicateVlans = CacheService.GetBool(_settings, "vlanDuplicate");
            Input.DecodeMACVendor = CacheService.GetBool(_settings, "decodeMAC");
            Input.Vaults = CacheService.GetBool(_settings, "enableVaults");
            Input.Vaults = CacheService.GetBool(_settings, "passkeys");

            #endregion

            //await loadingModal.HideAsync();

            StateHasChanged();
        }

        private async Task SaveSettings()
        {
            Dictionary<string, string> currentSettings = new Dictionary<string, string>
            {
                { "siteTitle",             Input.SiteTitle },
                { "siteDomain",            Input.SiteDomain },
                { "siteURL",               Input.SiteUrl },
                { "siteLoginText",         Input.LoginText },
                { "permissionPropagate",   Input.PermissionPropagation.ToString() },
                { "vlanMax",               Input.MaxVLAN.ToString() },
                { "maintaneanceMode",      Input.MaintenanceMode.ToString() },
                { "siteAdminName",         Input.AdminName },
                { "siteAdminMail",         Input.AdminEmail },
                { "api",                   Input.API.ToString() },
                { "enableIPrequests",      Input.IPRequestModule.ToString() },
                { "enableVRF",             Input.VRFSupport.ToString() },
                { "enableNAT",             Input.NAT.ToString() },
                { "enablePowerDNS",        Input.PowerDNS.ToString() },
                { "enableDHCP",            Input.DHCP.ToString() },
                { "enableFirewallZones",   Input.FirewallZones.ToString() },
                { "enableDNSresolving",    Input.ResolveDNS.ToString() },
                { "tempShare",             Input.TemporaryShares.ToString() },
                { "enableChangelog",       Input.Changelog.ToString() },
                { "enableMulticast",       Input.MulticastModule.ToString() },
                { "enableThreshold",       Input.ThresholdModule.ToString() },
                { "enableRACK",            Input.RackModule.ToString() },
                { "enableCircuits",        Input.CircuitsModule.ToString() },
                { "enableLocations",       Input.LocationsModule.ToString() },
                { "enableSNMP",            Input.SNMPModule.ToString() },
                { "enablePSNT",            Input.PSTNModule.ToString() },
                { "enableCustomers",       Input.CustomersModule.ToString() },
                { "enableRouting",         Input.RoutingModule.ToString() },
                { "updateTags",            Input.UpdateTags.ToString() },
                { "enforceUnique",         Input.RequireUniqueSubnets.ToString() },
                { "vlanDuplicate",         Input.AllowDuplicateVlans.ToString() },
                { "decodeMAC",             Input.DecodeMACVendor.ToString() },
                { "enableVaults",          Input.Vaults.ToString() },
                { "passkeys",              Input.Passkeys.ToString() },
            };

            List<KeyValuePair<string, string>> changedSettings = currentSettings
                .Where(kv => !_originalSettings.TryGetValue(kv.Key, out string? original) || !original.Equals(kv.Value, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!changedSettings.Any())
                return; // Nothing to save

            using AppDbContext db = DbFactory.CreateDbContext();

            foreach (KeyValuePair<string, string> changed in changedSettings)
            {
                Setting setting = await db.Settings.FirstAsync(s => s.Name == changed.Key);
                setting.Value = changed.Value;
            }

            await db.SaveChangesAsync();

            _originalSettings = new Dictionary<string, string>(currentSettings);
            CacheService.InvalidateCache("app_settings");
        }

        private sealed class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            public string SiteTitle { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Text)]
            public string SiteDomain { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Url)]
            public string SiteUrl { get; set; } = string.Empty;

            [DataType(DataType.Text)]
            public string LoginText { get; set; } = string.Empty;

            [Required]
            public bool PermissionPropagation { get; set; } = false;

            [Required]
            [Range(1, 4096)]
            public int MaxVLAN { get; set; } = 4096;

            [Required]
            public bool MaintenanceMode { get; set; } = false;

            [Required]
            [DataType(DataType.Text)]
            public string AdminName { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.EmailAddress)]
            public string AdminEmail { get; set; } = string.Empty;

            [Required]
            public bool API { get; set; } = false;

            [Required]
            public bool IPRequestModule { get; set; } = false;

            [Required]
            public bool VRFSupport { get; set; } = false;

            [Required]
            public bool NAT { get; set; } = false;

            [Required]
            public bool PowerDNS { get; set; } = false;

            [Required]
            public bool DHCP { get; set; } = false;

            [Required]
            public bool FirewallZones { get; set; } = false;

            [Required]
            public bool ResolveDNS { get; set; } = false;

            [Required]
            public bool TemporaryShares { get; set; } = false;

            [Required]
            public bool Changelog { get; set; } = false;

            [Required]
            public bool MulticastModule { get; set; } = false;

            [Required]
            public bool ThresholdModule { get; set; } = false;

            [Required]
            public bool RackModule { get; set; } = false;

            [Required]
            public bool CircuitsModule { get; set; } = false;

            [Required]
            public bool LocationsModule { get; set; } = false;

            [Required]
            public bool SNMPModule { get; set; } = false;

            [Required]
            public bool PSTNModule { get; set; } = false;

            [Required]
            public bool CustomersModule { get; set; } = false;

            [Required]
            public bool RoutingModule { get; set; } = false;

            [Required]
            public bool UpdateTags { get; set; } = false;

            [Required]
            public bool RequireUniqueSubnets { get; set; } = false;

            [Required]
            public bool AllowDuplicateVlans { get; set; } = false;

            [Required]
            public bool DecodeMACVendor { get; set; } = false;

            [Required]
            public bool Vaults { get; set; } = false;

            [Required]
            public bool Passkeys { get; set; } = false;
        }
    }
}
