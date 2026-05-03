namespace netIPAM.Components.Pages.Subnets
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private SubnetService Subnets { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? SectionId { get; set; }
        [Parameter] public int? Id { get; set; }
        
        private Subnet? _subnet;
        private FormModel _form = new();
        private string? _error;
        
        private bool _isNew => Id is null;

        private string CancelHref => SectionId is not null
            ? $"/sections/{SectionId}/subnets"
            : (_subnet?.SectionId is int sid ? $"/sections/{sid}/subnets" : "/sections");

        protected override async Task OnParametersSetAsync()
        {
            if (_isNew)
            {
                _subnet = new Subnet { SectionId = SectionId };
                _form = new FormModel();
            }
            else
            {
                _subnet = await Subnets.GetAsync(Id!.Value);
                if (_subnet is null) { Nav.NavigateTo("/sections"); return; }
                _form = new FormModel
                {
                    Cidr = TryFormatCidr(_subnet)
                };
            }
        }

        private static string TryFormatCidr(Subnet s)
        {
            if (string.IsNullOrEmpty(s.SubnetAddress) || string.IsNullOrEmpty(s.Mask)) 
                return string.Empty;
            try
            {
                IpVersion v = IpConverter.GuessVersion(s.SubnetAddress);
                return $"{IpConverter.ToPresentation(s.SubnetAddress, v)}/{s.Mask}";
            }
            catch { return $"{s.SubnetAddress}/{s.Mask}"; }
        }

        private async Task Save()
        {
            if (_subnet is null) return;
            _error = null;

            if (!_subnet.IsFolder)
            {
                if (string.IsNullOrWhiteSpace(_form.Cidr) || !_form.Cidr.Contains('/'))
                {
                    _error = "Please provide CIDR notation (e.g. 10.0.0.0/8).";
                    return;
                }

                string[] parts = _form.Cidr.Trim().Split('/');
                try
                {
                    string dec = IpConverter.ToDecimal(parts[0]);
                    if (!int.TryParse(parts[1], out var mask)) { _error = "Invalid mask."; return; }
                    _subnet.SubnetAddress = dec;
                    _subnet.Mask = mask.ToString();
                }
                catch (Exception ex) { _error = $"Invalid CIDR: {ex.Message}"; return; }
            }
            else
            {
                _subnet.SubnetAddress = "0";
                _subnet.Mask = "";
            }

            if (_isNew) await Subnets.CreateAsync(_subnet);
            else await Subnets.UpdateAsync(_subnet);

            Nav.NavigateTo(_subnet.SectionId is int sid ? $"/sections/{sid}/subnets" : "/sections");
        }

        public class FormModel
        {
            public string Cidr { get; set; } = string.Empty;
        }
    }
}
