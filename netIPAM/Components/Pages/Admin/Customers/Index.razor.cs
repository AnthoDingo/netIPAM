namespace netIPAM.Components.Pages.Admin.Customers
{
    public partial class Index : LocalizedComponentBase
    {
        [Inject]
        private CustomerService Customers { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private List<Customer>? _customers;
        protected override async Task OnInitializedAsync() => await Reload();
        private async Task Reload() => _customers = await Customers.ListAsync();
        private async Task Delete(int id) { await Customers.DeleteAsync(id); await Reload(); }

        private static MarkupString StatusBadge(string? s) => s switch
        {
            "Active" => new("<span class=\"badge bg-success\">Active</span>"),
            "Reserved" => new("<span class=\"badge bg-warning text-dark\">Reserved</span>"),
            "Inactive" => new("<span class=\"badge bg-secondary\">Inactive</span>"),
            _ => new("—")
        };
    }
}
