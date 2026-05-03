namespace netIPAM.Components.Pages.Setup
{
    public partial class Index
    {
        [Inject]
        private SetupService SetupSvc { get; set; } = default!;

        [Inject]
        private SetupState SetupState { get; set; } = default!;

        [Inject]
        private IHostApplicationLifetime Lifetime { get; set; } = default!;

        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        private static readonly string[] Steps = ["Bienvenue", "Base de données", "Schéma", "Administrateur", "Terminé"];

        private int _step = 0;
        private string _provider = "Sqlite";
        private string _connectionString = "Data Source=netipam.db";

        // Test connexion
        private bool _testing;
        private TestConnectionResult? _testResult;

        // Migrations
        private bool _migrationsStarted;
        private bool _migrationsRunning;
        private bool _migrationsOk;
        private List<string> _migrationLog = [];

        // Admin
        private string _adminUsername = "Admin";
        private string _adminEmail = string.Empty;
        private string _adminPassword = string.Empty;
        private string _adminPassword2 = string.Empty;
        private string? _adminError;
        private bool _creatingAdmin;

        // Countdown
        private int _countdown = 8;
        private System.Threading.Timer? _timer;

        // ──────────────────────────────────────────────────────────────

        private void SelectProvider(string p)
        {
            _provider = p;
            _testResult = null;
            _connectionString = p == "SqlServer"
                ? "Server=(localdb)\\MSSQLLocalDB;Database=netIPAM;Trusted_Connection=True;TrustServerCertificate=True"
                : "Data Source=netipam.db";
        }

        private async Task TestConnection()
        {
            _testing = true;
            _testResult = null;
            StateHasChanged();
            _testResult = await SetupSvc.TestConnectionAsync(_provider, _connectionString);
            _testing = false;
        }

        private void GoToMigrations()
        {
            if (_testResult?.Success == true)
            {
                _step = 2;
                _migrationsStarted = false;
                _migrationsOk = false;
                _migrationLog = [];
            }
        }

        private async Task ApplyMigrations()
        {
            _migrationsStarted = true;
            _migrationsRunning = true;
            _migrationsOk = false;
            _migrationLog = [];

            await foreach (string line in SetupSvc.ApplyMigrationsAsync(_provider, _connectionString))
            {
                _migrationLog.Add(line);
                StateHasChanged();
                await Task.Delay(50); // effet visuel ligne par ligne
            }

            _migrationsRunning = false;
            _migrationsOk = !_migrationLog.Any(l => l.StartsWith("❌"));
            StateHasChanged();
        }

        private async Task RetryMigrations()
        {
            _migrationsStarted = false;
            await Task.Delay(100);
            await ApplyMigrations();
        }

        private async Task CreateAdminAndFinish()
        {
            if (string.IsNullOrWhiteSpace(_adminUsername))
            {
                _adminError = "Le nom d'utilisateur est requis.";
                return;
            }
            if (string.IsNullOrWhiteSpace(_adminPassword))
            {
                _adminError = "Le mot de passe est requis.";
                return;
            }
            if (_adminPassword != _adminPassword2)
            {
                _adminError = "Les mots de passe ne correspondent pas.";
                return;
            }

            _adminError = null;
            _creatingAdmin = true;
            StateHasChanged();

            // Créer le compte admin
            string? error = await SetupSvc.CreateAdminAsync(
                _provider, _connectionString,
                _adminUsername, _adminEmail, _adminPassword);

            if (error is not null)
            {
                _adminError = error;
                _creatingAdmin = false;
                return;
            }

            // Écrire appsettings.json
            SetupSvc.WriteAppSettings(_provider, _connectionString);

            // Marquer comme terminé en mémoire (plus de redirection)
            SetupState.MarkCompleted();

            _step = 4;
            _creatingAdmin = false;
            StateHasChanged();

            // Countdown + arrêt de l'application (redémarrage par le superviseur)
            _timer = new System.Threading.Timer(_ =>
            {
                _countdown--;
                InvokeAsync(StateHasChanged);
                if (_countdown <= 0)
                {
                    _timer?.Dispose();
                    Lifetime.StopApplication();
                }
            }, null, 1000, 1000);
        }

        // Indicateur de force de mot de passe
        private (string css, int pct, string label) PasswordStrength(string pwd)
        {
            int score = 0;
            if (pwd.Length >= 8) score++;
            if (pwd.Length >= 12) score++;
            if (pwd.Any(char.IsDigit)) score++;
            if (pwd.Any(char.IsUpper) && pwd.Any(char.IsLower)) score++;
            if (pwd.Any(c => "!@#$%^&*()-_=+[]{}|;:,.<>?".Contains(c))) score++;

            return score switch
            {
                <= 1 => ("bg-danger", 25, "Faible"),
                2 => ("bg-warning", 50, "Moyen"),
                3 => ("bg-info", 75, "Bon"),
                _ => ("bg-success", 100, "Fort"),
            };
        }

        private void SetDefaultSqliteConnection()
        {
            _connectionString = "Data Source=netipam.db";
        }

        public void Dispose() => _timer?.Dispose();
    }
}
