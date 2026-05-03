namespace netIPAM.Components.Pages.Admin.IpTags
{
    public partial class Index : LocalizedComponentBase
    {
        [Inject]
        private IpTagService Tags { get; set; } = default!;
        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<IpTag>? _tags;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _tags = await Tags.ListAsync();
        private async Task Delete(int id) { await Tags.DeleteAsync(id); await Reload(); }
    }
}
