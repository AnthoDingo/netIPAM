namespace netIPAM.Components.Pages.Subnets
{
    public partial class View : LocalizedComponentBase
    {
        [Inject]
        private SubnetService Subnets { get; set; } = default!;

        [Inject]
        private IpAddressService Ips { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int Id { get; set; }
        
        private Subnet? _subnet;
        private List<IpAddress>? _addresses;
        private SubnetCalculator.SubnetInfo? _info;
        private (long total, long used, double percentUsed) _usage;
        private string _cidr = string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            _subnet = await Subnets.GetAsync(Id);
            if (_subnet is null) { Nav.NavigateTo("/sections"); return; }

            if (!string.IsNullOrEmpty(_subnet.SubnetAddress) && int.TryParse(_subnet.Mask, out var bits))
            {
                try
                {
                    _info = SubnetCalculator.Describe(_subnet.SubnetAddress, bits);
                    _cidr = $"{_info.Network}/{bits}";
                }
                catch { _cidr = $"{_subnet.SubnetAddress}/{_subnet.Mask}"; }
            }

            _addresses = await Ips.ForSubnetAsync(Id);
            _usage = await Subnets.UsageAsync(Id);
        }

        private static string FormatIp(IpAddress ip)
        {
            try
            {
                IpVersion v = IpConverter.GuessVersion(ip.IpAddr);
                return IpConverter.ToPresentation(ip.IpAddr, v);
            }
            catch { return ip.IpAddr; }
        }

        private static MarkupString StateBadge(int? state) => state switch
        {
            0 => new("<span class='phpipam-state phpipam-state-offline'>Offline</span>"),
            1 => new("<span class='phpipam-state phpipam-state-active'>Active</span>"),
            2 => new("<span class='phpipam-state phpipam-state-used'>Used</span>"),
            3 => new("<span class='phpipam-state phpipam-state-reserved'>Reserved</span>"),
            4 => new("<span class='phpipam-state phpipam-state-dhcp'>DHCP</span>"),
            _ => new("")
        };

        private async Task DeleteIp(int ipId)
        {
            await Ips.DeleteAsync(ipId);
            _addresses = await Ips.ForSubnetAsync(Id);
            _usage = await Subnets.UsageAsync(Id);
        }
    }
}
