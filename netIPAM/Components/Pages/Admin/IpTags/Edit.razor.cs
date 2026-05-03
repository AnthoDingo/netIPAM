namespace netIPAM.Components.Pages.Admin.IpTags
{
    public partial class Edit
    {
        [Inject]
        private IpTagService Tags { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private IpTag? _tag;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _tag = _isNew ? new IpTag { ShowTag = 1, BgColor = "#3498db", FgColor = "#ffffff", Compress = "No", Locked = "No" }
                         : await Tags.GetAsync(Id!.Value);
            if (_tag is null) Nav.NavigateTo("/admin/iptags");
        }

        private async Task Save()
        {
            if (_tag is null) return;
            if (_isNew) await Tags.CreateAsync(_tag); else await Tags.UpdateAsync(_tag);
            Nav.NavigateTo("/admin/iptags");
        }
    }
}
