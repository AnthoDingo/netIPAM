namespace netIPAM.Components.Pages.Admin.Customers
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private CustomerService Customers { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private Customer? _customer;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _customer = _isNew ? new Customer { Status = "Active" } : await Customers.GetAsync(Id!.Value);
            if (_customer is null) Nav.NavigateTo("/admin/customers");
        }

        private async Task Save()
        {
            if (_customer is null) return;
            if (_isNew) await Customers.CreateAsync(_customer); else await Customers.UpdateAsync(_customer);
            Nav.NavigateTo("/admin/customers");
        }
    }
}
