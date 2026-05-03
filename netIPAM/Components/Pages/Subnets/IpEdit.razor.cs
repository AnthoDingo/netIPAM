namespace netIPAM.Components.Pages.Subnets
{
    public partial class IpEdit : LocalizedComponentBase
    {
        [Inject]
        private IpAddressService Ips { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int SubnetId { get; set; }
        [Parameter] public int? Id { get; set; }
        
        private IpAddress? _ip;
        private FormModel _form = new();
        private string? _error;
        
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            if (_isNew)
            {
                _ip = new IpAddress { SubnetId = SubnetId, State = 2 };
            }
            else
            {
                _ip = await Ips.GetAsync(Id!.Value);
                if (_ip is null) { Nav.NavigateTo($"/subnets/{SubnetId}"); return; }
                try
                {
                    IpVersion v = IpConverter.GuessVersion(_ip.IpAddr);
                    _form.Address = IpConverter.ToPresentation(_ip.IpAddr, v);
                }
                catch { _form.Address = _ip.IpAddr; }
            }
        }

        private async Task Save()
        {
            if (_ip is null) return;
            _error = null;
            
            if (string.IsNullOrWhiteSpace(_form.Address)) 
            { 
                _error = "IP required."; 
                return; 
            }
            
            try 
            { 
                _ip.IpAddr = IpConverter.ToDecimal(_form.Address); 
            }
            catch (Exception ex) 
            { 
                _error = ex.Message; 
                return; 
            }

            if (_isNew) await Ips.CreateAsync(_ip);
            else await Ips.UpdateAsync(_ip);

            Nav.NavigateTo($"/subnets/{SubnetId}");
        }

        public class FormModel 
        { 
            public string Address { get; set; } = string.Empty; 
        }
    }
}
