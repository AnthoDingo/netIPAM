using netIPAM.Enums;

namespace netIPAM.Components.Pages.Admin.Devices
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private DeviceService Devices { get; set; } = default!;
        [Inject]
        private DeviceTypeService Types { get; set; } = default!;
        [Inject]
        private LocationService Locations { get; set; } = default!;
        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        [SupplyParameterFromForm] private FormModel _form { get; set; } = new();
        private Device? _device;
        private List<DeviceType>? _types;
        private List<Location>? _locations;
        private string? _error;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _types = await Types.ListAsync();
            _locations = await Locations.ListAsync();
            if (_isNew) _device = new Device { Type = 0, SnmpVersion = "0" };
            else
            {
                _device = await Devices.GetAsync(Id!.Value);
                if (_device is null) { Nav.NavigateTo("/admin/devices"); return; }
                try { IpVersion v = IpConverter.GuessVersion(_device.IpAddr ?? "0"); _form.IpPresentation = string.IsNullOrEmpty(_device.IpAddr) ? "" : IpConverter.ToPresentation(_device.IpAddr, v); }
                catch { _form.IpPresentation = _device.IpAddr ?? ""; }
            }
        }

        private async Task Save()
        {
            if (_device is null) return;
            _error = null;
            if (!string.IsNullOrWhiteSpace(_form.IpPresentation))
            {
                try { _device.IpAddr = IpConverter.ToDecimal(_form.IpPresentation); }
                catch (Exception ex) { _error = ex.Message; return; }
            }
            else _device.IpAddr = null;

            if (_isNew) await Devices.CreateAsync(_device); else await Devices.UpdateAsync(_device);
            Nav.NavigateTo("/admin/devices");
        }

        public class FormModel { public string IpPresentation { get; set; } = string.Empty; }
    }
}
