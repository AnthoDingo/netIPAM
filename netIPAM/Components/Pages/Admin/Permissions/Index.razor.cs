namespace netIPAM.Components.Pages.Admin.Permissions
{
    public partial class Index
    {
        [Inject]
        private PermissionService PermSvc { get; set; } = default!;

        [Inject]
        private UserService Users { get; set; } = default!;

        [Inject]
        private UserGroupService Groups { get; set; } = default!;

        [Inject]
        private VlanService Vlans { get; set; } = default!;

        [Inject]
        private VlanDomainService L2Domains { get; set; } = default!;

        [Inject]
        private VrfService Vrfs { get; set; } = default!;

        [Inject]
        private DeviceService Devices { get; set; } = default!;


        record TabDef(string Key, string Label, string Icon);
        record ResourceRow(int EntityId, string Name, string Detail);

        private readonly List<TabDef> _tabs =
        [
            new(EntityTypes.Vlan,     "VLANs",      "bi bi-tag"),
        new(EntityTypes.L2Domain, "L2 Domains", "bi bi-layers"),
        new(EntityTypes.Vrf,      "VRFs",        "bi bi-share"),
        new(EntityTypes.Device,   "Devices",     "bi bi-router"),
    ];

        // Données chargées au démarrage
        private List<UserGroup> _groups = [];
        private List<User> _users = [];
        private Dictionary<(string, int), Dictionary<string, int>> _subjectCounts = new();

        // Ressources par type
        private List<Vlan> _vlans = [];
        private List<VlanDomain> _l2Doms = [];
        private List<Vrf> _vrfs = [];
        private List<Device> _devs = [];

        // Sujet sélectionné
        private string? _selectedSubjectType;
        private int _selectedSubjectId;
        private string? _selectedSubjectName;
        private List<int> _userGroupIds = [];

        // Draft (onglet actif)
        private string _activeTab = EntityTypes.Vlan;
        private Dictionary<(string, int), int> _draft = new();
        private Dictionary<string, int> _draftCounts = new();
        private bool _loading;
        private bool _saved;

        protected override async Task OnInitializedAsync()
        {
            _groups = await Groups.ListAsync();
            _users = await Users.ListAsync();
            _vlans = await Vlans.ListAsync();
            _l2Doms = await L2Domains.ListAsync();
            _vrfs = await Vrfs.ListAsync();
            _devs = await Devices.ListAsync();

            // Pré-calculer les compteurs par sujet
            foreach (UserGroup g in _groups)
            {
                Dictionary<string, int> counts = await PermSvc.GetPermissionCountsAsync(SubjectTypes.Group, g.GId);
                if (counts.Values.Sum() > 0) _subjectCounts[(SubjectTypes.Group, g.GId)] = counts;
            }
            foreach (User u in _users)
            {
                Dictionary<string, int> counts = await PermSvc.GetPermissionCountsAsync(SubjectTypes.User, u.Id);
                if (counts.Values.Sum() > 0) _subjectCounts[(SubjectTypes.User, u.Id)] = counts;
            }
        }

        private bool IsSelected(string type, int id)
            => _selectedSubjectType == type && _selectedSubjectId == id;

        private async Task SelectSubject(string type, int id, string name)
        {
            _selectedSubjectType = type;
            _selectedSubjectId = id;
            _selectedSubjectName = name;
            _activeTab = EntityTypes.Vlan;
            _saved = false;

            if (type == SubjectTypes.User)
            {
                User? u = await Users.FindByIdAsync(id);
                _userGroupIds = u is not null ? Users.GetGroupIds(u) : [];
            }
            else _userGroupIds = [];

            await LoadPermissionsForSubject();
        }

        private async Task LoadPermissionsForSubject()
        {
            _loading = true; StateHasChanged();
            _draft.Clear();

            foreach (TabDef tab in _tabs)
            {
                List<EntityPermission> perms = await PermSvc.GetForSubjectAsync(
                    _selectedSubjectType!, _selectedSubjectId, tab.Key);
                foreach (EntityPermission p in perms)
                    _draft[(tab.Key, p.EntityId)] = p.Level;
            }

            RefreshDraftCounts();
            _loading = false;
        }

        private void SetDraft(int entityId, int level)
        {
            (string, int) key = (_activeTab, entityId);
            if (level == 0) _draft.Remove(key);
            else _draft[key] = level;
            RefreshDraftCounts();
        }

        private void RefreshDraftCounts()
        {
            _draftCounts = _tabs.ToDictionary(
                t => t.Key,
                t => _draft.Keys.Count(k => k.Item1 == t.Key));
        }

        private async Task SaveCurrentTab()
        {
            IEnumerable<(int, int)> perms = GetCurrentRows()
                .Select(r => (r.EntityId, _draft.TryGetValue((_activeTab, r.EntityId), out var l) ? l : 0));

            await PermSvc.SetBulkAsync(
                _selectedSubjectType!, _selectedSubjectId, _activeTab, perms);

            // Rafraîchir le compteur global
            Dictionary<string, int> counts = await PermSvc.GetPermissionCountsAsync(_selectedSubjectType!, _selectedSubjectId);
            if (counts.Values.Sum() > 0)
                _subjectCounts[(_selectedSubjectType!, _selectedSubjectId)] = counts;
            else
                _subjectCounts.Remove((_selectedSubjectType!, _selectedSubjectId));

            _saved = true;
            StateHasChanged();
            await Task.Delay(2000);
            _saved = false;
        }

        private async Task ClearCurrentTab()
        {
            foreach (ResourceRow row in GetCurrentRows())
                _draft.Remove((_activeTab, row.EntityId));
            RefreshDraftCounts();
            await SaveCurrentTab();
        }

        private List<ResourceRow> GetCurrentRows() => _activeTab switch
        {
            EntityTypes.Vlan => _vlans.Select(v => new ResourceRow(v.VlanId, v.Name, $"VLAN {v.Number}")).ToList(),
            EntityTypes.L2Domain => _l2Doms.Select(d => new ResourceRow(d.Id, d.Name ?? $"Domain {d.Id}", d.Description ?? "")).ToList(),
            EntityTypes.Vrf => _vrfs.Select(v => new ResourceRow(v.VrfId, v.Name, v.Rd ?? "")).ToList(),
            EntityTypes.Device => _devs.Select(d => new ResourceRow(d.Id, d.Hostname ?? $"Device {d.Id}", d.IpAddr ?? "")).ToList(),
            _ => []
        };
    }
}
