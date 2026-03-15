using Microsoft.AspNetCore.Components.Forms;
//using Microsoft.EntityFrameworkCore;
//using netIPAM.DBContexts;
//using netIPAM.Models;

namespace netIPAM.Pages.Administration
{
    public partial class Settings
    {

        [SupplyParameterFromForm(FormName = "settings")]
        private InputModel Input { get; set; } = default!;

        private EditContext editContext = default!;

        //[Inject]
        //private IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            editContext = new EditContext(Input);
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    await base.OnAfterRenderAsync(firstRender);
        //    if (firstRender)
        //    {
                

        //        using AppDbContext db = DbFactory.CreateDbContext();
        //        List<Setting> settings = await db.Settings.ToListAsync();

        //        Input.SiteTitle = settings.FirstOrDefault(s => s.Name == "siteTitle")?.Value ?? "";
        //        Input.SiteDomain = settings.FirstOrDefault(s => s.Name == "siteDomain")?.Value ?? "";
        //        Input.SiteUrl = settings.FirstOrDefault(s => s.Name == "siteURL")?.Value ?? "";
        //        Input.LoginText = settings.FirstOrDefault(s => s.Name == "siteLoginText")?.Value ?? "";

                
        //        StateHasChanged();
        //    }
            
        //}

        private sealed class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            public string SiteTitle { get; set; } = "";

            [Required]
            [DataType(DataType.Text)]
            public string SiteDomain { get; set; } = "";

            [Required]
            [DataType(DataType.Url)]
            public string SiteUrl { get; set; } = "";

            [DataType(DataType.Text)]
            public string LoginText { get; set; } = string.Empty;

            [Required]
            public bool PermissionPropagation { get; set; } = true;

            [Required]
            [Range(1, 4096)]
            public int MaxVLAN { get; set; } = 4096;

            [Required]
            public bool MaintenanceMode { get; set; } = false;

            [Required]
            [DataType(DataType.Text)]
            public string AdminName { get; set; } = "Sysadmin";

            [Required]
            [DataType(DataType.EmailAddress)]
            public string AdminEmail { get; set; } = "admin@domain.local";

            [Required]
            public bool API { get; set; } = false;

            [Required]
            public bool IPRequestModule { get; set; } = true;

            [Required]
            public bool VRFSupport { get; set; } = false;

            [Required]
            public bool NAT { get; set; } = true;

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
            public bool Changelog { get; set; } = true;

            [Required]
            public bool MulticastModule { get; set; } = false;

            [Required]
            public bool ThresholdModule { get; set; } = true;

            [Required]
            public bool RackModule { get; set; } = true;

            [Required]
            public bool CircuitsModule { get; set; } = true;

            [Required]
            public bool LocationsModule { get; set; } = true;

            [Required]
            public bool SNMPModule { get; set; } = false;

            [Required]
            public bool PSTNModule { get; set; } = false;

            [Required]
            public bool CustomersModule { get; set; } = true;

            [Required]
            public bool RoutingModule { get; set; } = false;

            [Required]
            public bool UpdateTags { get; set; } = false;

            [Required]
            public bool RequireUniqueSubnets { get; set; } = true;

            [Required]
            public bool AllowDuplicateVlans { get; set; } = true;

            [Required]
            public bool DecodeMACVendor { get; set; } = true;

            [Required]
            public bool Vaults { get; set; } = true;

            [Required]
            public bool Passkeys { get; set; } = true;
        }
    }
}
