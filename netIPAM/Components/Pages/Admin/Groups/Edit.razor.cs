namespace netIPAM.Components.Pages.Admin.Groups
{
    public partial class Edit : LocalizedComponentBase
    {
        [Inject]
        private UserGroupService Groups { get; set; } = default!;

        [Inject]
        private UserService Users { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        [Parameter] public int? Id { get; set; }
        private UserGroup? _group;
        private List<User> _allUsers = [];
        private List<User> _members = [];
        private string? _memberMsg;
        private bool _isNew => Id is null;

        protected override async Task OnParametersSetAsync()
        {
            _group = _isNew ? new UserGroup() : await Groups.GetAsync(Id!.Value);
            if (_group is null) { Nav.NavigateTo("/admin/groups"); return; }
            if (!_isNew) await ReloadMembers();
        }

        private async Task ReloadMembers()
        {
            _allUsers = await Users.ListAsync();
            _members = await Users.GetMembersOfGroupAsync(Id!.Value);
        }

        private async Task AddMember(int userId)
        {
            User? user = await Users.FindByIdAsync(userId);
            if (user is null) return;
            List<int> groupIds = Users.GetGroupIds(user);
            if (!groupIds.Contains(Id!.Value)) groupIds.Add(Id.Value);
            await Users.SetGroupIdsAsync(userId, groupIds);
            _memberMsg = $"✓ Membre ajouté.";
            await ReloadMembers();
        }

        private async Task RemoveMember(int userId)
        {
            User? user = await Users.FindByIdAsync(userId);
            if (user is null) return;
            List<int> groupIds = Users.GetGroupIds(user).Where(g => g != Id!.Value).ToList();
            await Users.SetGroupIdsAsync(userId, groupIds);
            _memberMsg = $"✓ Membre retiré.";
            await ReloadMembers();
        }

        private async Task Save()
        {
            if (_group is null) return;
            if (_isNew) await Groups.CreateAsync(_group); else await Groups.UpdateAsync(_group);
            Nav.NavigateTo("/admin/groups");
        }
    }
}
