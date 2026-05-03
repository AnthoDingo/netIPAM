namespace netIPAM.Components.Pages.Admin.DeviceTypes
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private DeviceTypeService Types { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private DeviceType? _type;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _type = _isNew ? new DeviceType() : await Types.GetAsync(Id!.Value);
            if (_type is null) Nav.NavigateTo("/admin/devicetypes");
        }

        private async Task Save()
        {
            if (_type is null) return;
            if (_isNew) await Types.CreateAsync(_type); else await Types.UpdateAsync(_type);
            Nav.NavigateTo("/admin/devicetypes");
        }
    }
}
