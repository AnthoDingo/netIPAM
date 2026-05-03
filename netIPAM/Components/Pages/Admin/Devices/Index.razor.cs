using netIPAM.Enums;

namespace netIPAM.Components.Pages.Admin.Devices
{
    public partial class Index
    {
        [Inject]
        private DeviceService Devices { get; set; } = default!;
        [Inject]
        private DeviceTypeService Types { get; set; } = default!;
        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<Device>? _devices;
        private Dictionary<int, string> _types = new();
        private Func<int, string> _typeName => id => _types.GetValueOrDefault(id, "—");

        protected override async Task OnInitializedAsync()
        {
            List<DeviceType> types = await Types.ListAsync();
            _types = types.ToDictionary(t => t.Tid, t => t.Tname ?? "—");
            await Reload();
        }

        private async Task Reload() => _devices = await Devices.ListAsync();
        private async Task Delete(int id) { await Devices.DeleteAsync(id); await Reload(); }

        private static string FormatIp(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";
            try { IpVersion v = IpConverter.GuessVersion(raw); return IpConverter.ToPresentation(raw, v); }
            catch { return raw; }
        }
    }
}
