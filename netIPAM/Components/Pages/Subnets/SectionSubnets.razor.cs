namespace netIPAM.Components.Pages.Subnets
{
    public partial class SectionSubnets
    {
        [Inject]
        private SectionService Sections { get; set; } = default!;

        [Inject]
        private SubnetService Subnets { get; set; } = default!;

        [Parameter] public int SectionId { get; set; }
        
        private Section? _section;
        private List<Subnet>? _subnets;
        private Dictionary<int, (long total, long used, double percentUsed)> _usage = new();

        protected override async Task OnParametersSetAsync()
        {
            _section = await Sections.GetAsync(SectionId);
            if (_section is null) return;
            
            _subnets = await Subnets.ForSectionAsync(SectionId);
            _usage.Clear();
            
            foreach (Subnet s in _subnets.Where(x => !x.IsFolder))
                _usage[s.Id] = await Subnets.UsageAsync(s.Id);
        }

        /// <summary>
        /// Rendu récursif des lignes de subnet avec indentation basée sur la profondeur.
        /// Supporte les subnets parents/enfants et les dossiers logiques.
        /// </summary>
        private RenderFragment RenderSubnetRow(Subnet s, int depth) => __builder =>
        {
            string cidr = FormatCidr(s);
            (long total, long used, double percentUsed) u = _usage.GetValueOrDefault(s.Id);
            int pad = depth * 20;

            <tr>
                <td style="padding-left: @(12 + pad)px">
                    @if (s.IsFolder)
                    {
                        <i class="bi bi-folder-fill me-1 text-warning"></i>
                        <span>@(s.Description ?? "Folder")</span>
                    }
                    else
                    {
                        <a href="@($"/subnets/{s.Id}")" class="text-decoration-none fw-500">
                            @if (depth > 0) 
                            { 
                                <i class="bi bi-chevron-right me-1 text-muted" style="font-size:10px"></i> 
                            }
                            <i class="bi bi-diagram-3 me-1 text-muted"></i>@cidr
                        </a>
                    }
                </td>
                <td class="text-muted small">@s.Description</td>
                <td>
                    @if (s.Vlan is not null)
                    {
                        <span class="badge bg-warning text-dark">@s.Vlan.Name (@s.Vlan.Number)</span>
                    }
                </td>
                <td style="width: 180px;">
                    @if (!s.IsFolder && u.total > 0)
                    {
                        double pct = u.percentUsed;
                        string cls = pct >= 90 ? "crit" : pct >= 70 ? "warn" : "";
                        <div class="phpipam-usage">
                            <div class="phpipam-usage-fill @cls" style="width: @($"{pct:0.#}")%"></div>
                            <div class="phpipam-usage-label">@u.used / @u.total</div>
                        </div>
                    }
                </td>
                <td class="text-end">
                    @if (!s.IsFolder)
                    {
                        <a class="btn btn-sm btn-outline-secondary" href="@($"/subnets/{s.Id}")">
                            <i class="bi bi-eye"></i>
                        </a>
                    }
                    <a class="btn btn-sm btn-outline-secondary" href="@($"/subnets/{s.Id}/edit")">
                        <i class="bi bi-pencil"></i>
                    </a>
                    <button class="btn btn-sm btn-outline-danger" @onclick="() => Delete(s.Id)">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>

            @* Enfants récursifs *@
            @if (_subnets is not null)
            {
                foreach (Subnet child in _subnets.Where(x => x.MasterSubnetId == s.Id))
                {
                    @RenderSubnetRow(child, depth + 1)
                }
            }
        };

        private async Task Delete(int id)
        {
            await Subnets.DeleteAsync(id);
            _subnets = await Subnets.ForSectionAsync(SectionId);
        }

        /// <summary>
        /// Formatte un subnet en notation CIDR. Supporte IPv4 et IPv6.
        /// </summary>
        private static string FormatCidr(Subnet s)
        {
            if (string.IsNullOrEmpty(s.SubnetAddress) || string.IsNullOrEmpty(s.Mask)) 
                return "—";
            
            try
            {
                IpVersion v = IpConverter.GuessVersion(s.SubnetAddress);
                return $"{IpConverter.ToPresentation(s.SubnetAddress, v)}/{s.Mask}";
            }
            catch 
            { 
                return $"{s.SubnetAddress}/{s.Mask}"; 
            }
        }
    }
}
