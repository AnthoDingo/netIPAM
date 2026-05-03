# Analyse de Refactorisation - Séparation du Code Razor (.razor.cs)

## 📋 Résumé Exécutif

Le projet `netIPAM` contient **14 fichiers .razor** avec du code C# mélangé (blocs `@code`). 
Le dossier `Pages/Admin` utilise déjà le pattern correct avec fichiers `.razor.cs` séparés.
Cette analyse recommande d'appliquer le même pattern à tous les autres fichiers.

---

## 🔍 Fichiers à Refactoriser

### **Groupe 1: Pages/Subnets (PRIORITÉ HAUTE)**
À refactoriser car contient de la logique métier significative.

| Fichier | Taille Code | Complexité | Status |
|---------|------------|-----------|--------|
| `Edit.razor` | 80 lignes | Moyenne | ❌ À séparer |
| `View.razor` | 213 lignes | **Élevée** | ❌ À séparer |
| `IpEdit.razor` | 116 lignes | Moyenne | ❌ À séparer |
| `SectionSubnets.razor` | 178 lignes | **Élevée** | ❌ À séparer |

**Raison:** Ces pages contiennent:
- Injection de dépendances (`@inject`)
- Paramètres de route (`[Parameter]`)
- Logique d'initialisation (`OnParametersSetAsync`)
- Méthodes métier complexes
- Modèles internes (`FormModel`, `SubnetInfo`)

---

### **Groupe 2: Pages/Sections (PRIORITÉ MOYENNE)**
Pages principales avec logique métier.

| Fichier | Contenu | Status |
|---------|---------|--------|
| `Index.razor` | Listage des sections | ❌ À séparer |
| `Edit.razor` | Édition de section | ❌ À séparer |

---

### **Groupe 3: Pages Utilitaires (PRIORITÉ MOYENNE)**

| Fichier | Contenu | Status |
|---------|---------|--------|
| `Pages/Dashboard.razor` | Tableau de bord principal | ❌ À séparer |
| `Pages/Tools/IpCalculator.razor` | Calculatrice IP | ❌ À séparer |
| `Pages/Migrate/Index.razor` | Outil de migration | ❌ À séparer |

---

### **Groupe 4: Composants Partagés (PRIORITÉ BASSE)**
Petits composants, séparation optionnelle.

| Fichier | Lignes | Status |
|---------|--------|--------|
| `Shared/SectionTabs.razor` | Tabs | ⚠️ Optional |
| `Shared/SectionSidebar.razor` | Sidebar | ⚠️ Optional |
| `Shared/ScanPanel.razor` | Panel | ⚠️ Optional |
| `AdminPageHeader.razor` | Header | ⚠️ Optional |
| `RedirectToLogin.razor` | Redirect | ⚠️ Optional |

---

## 🎯 Plan d'Action Recommandé

### **Phase 1: Pages/Subnets (Immédiat)**
✅ Bénéfice maximal | ✅ Faible risque | ✅ Dépendances claires

1. ✂️ **Edit.razor.cs**: Extraire le code métier
2. ✂️ **View.razor.cs**: Extraire la logique de calcul et affichage
3. ✂️ **IpEdit.razor.cs**: Extraire la gestion des IPs
4. ✂️ **SectionSubnets.razor.cs**: Extraire la récursion et rendu

### **Phase 2: Pages/Sections (Semaine suivante)**
1. ✂️ **Index.razor.cs**
2. ✂️ **Edit.razor.cs**

### **Phase 3: Pages Utilitaires (Optionnel)**
1. ✂️ **Dashboard.razor.cs**
2. ✂️ **Pages/Tools/IpCalculator.razor.cs**
3. ✂️ **Pages/Migrate/Index.razor.cs**

### **Phase 4: Composants Partagés (Futur)**
Seulement si la complexité augmente.

---

## 📐 Structure Actuelle vs Cible

### ❌ Actuellement (Subnets)
```
Components/Pages/Subnets/
├── Edit.razor          (contient @code + @inject)
├── View.razor          (contient @code + @inject)
├── IpEdit.razor        (contient @code + @inject)
└── SectionSubnets.razor (contient @code + @inject)
```

### ✅ Structure Cible (comme Admin)
```
Components/Pages/Subnets/
├── Edit.razor          (markup uniquement)
├── Edit.razor.cs       (code métier)
├── View.razor          (markup uniquement)
├── View.razor.cs       (code métier)
├── IpEdit.razor        (markup uniquement)
├── IpEdit.razor.cs     (code métier)
├── SectionSubnets.razor (markup uniquement)
└── SectionSubnets.razor.cs (code métier)
```

---

## 🔧 Exemple de Refactorisation: Edit.razor

### **AVANT** (Mélangé)
```razor
@page "/sections/{SectionId:int}/subnets/new"
@page "/subnets/{Id:int}/edit"
@attribute [Authorize]
@inject SubnetService Subnets
@inject NavigationManager Nav

<PageTitle>@(_isNew ? "New" : "Edit")</PageTitle>

<!-- Markup -->
<div>
  @if (_subnet is not null) { <!-- ... --> }
</div>

@code {
    [Parameter] public int? SectionId { get; set; }
    [Parameter] public int? Id { get; set; }
    private Subnet? _subnet;
    private FormModel _form = new();
    private string? _error;
    
    private bool _isNew => Id is null;
    
    protected override async Task OnParametersSetAsync()
    {
        // ... logique d'initialisation
    }
    
    private async Task Save() { /* ... */ }
    
    public class FormModel { /* ... */ }
}
```

### **APRÈS** (Séparé)

**Edit.razor** (markup uniquement)
```razor
@page "/sections/{SectionId:int}/subnets/new"
@page "/subnets/{Id:int}/edit"
@attribute [Microsoft.AspNetCore.Authorization.Authorize]

<PageTitle>@(_isNew ? "New subnet" : "Edit subnet")</PageTitle>

<h1 class="h3 mb-4">
  <i class="bi bi-diagram-3 me-2"></i>
  @(_isNew ? "New subnet" : "Edit subnet")
</h1>

<div class="phpipam-card" style="max-width: 720px;">
  <div class="phpipam-card-body">
    @if (_subnet is not null)
    {
      <EditForm Model="_form" OnValidSubmit="Save">
        <!-- Champs du formulaire -->
      </EditForm>
    }
  </div>
</div>
```

**Edit.razor.cs** (code métier)
```csharp
namespace netIPAM.Components.Pages.Subnets
{
    public partial class Edit
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
                _form = new FormModel { Cidr = TryFormatCidr(_subnet) };
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
```

---

## ✨ Avantages de la Refactorisation

| Avantage | Impact |
|----------|--------|
| 📖 **Lisibilité** | Markup et code séparés, plus facile à naviguer |
| 🧪 **Testabilité** | Logique métier isolée, plus facile à tester |
| 🔍 **Maintenabilité** | Moins de défilement, structure claire |
| 🎯 **Cohérence** | Tous les fichiers suivent le même pattern |
| 📊 **IntelliSense** | Meilleure autocomplétion dans l'éditeur |
| 🔄 **Refactoring** | Plus facile à refactoriser sans casser le markup |

---

## ⚠️ Points d'Attention

### **Classe Partielle**
La classe `.razor.cs` doit être `public partial` :
```csharp
namespace netIPAM.Components.Pages.Subnets
{
    public partial class Edit  // ✅ IMPORTANT: partial
    {
        // Code métier
    }
}
```

### **Dépendances (@inject → [Inject])**
Convertir de `@inject` à `[Inject]` dans le `.cs`:
```csharp
// ❌ AVANT (dans .razor)
@inject SubnetService Subnets

// ✅ APRÈS (dans .razor.cs)
[Inject]
private SubnetService Subnets { get; set; } = default!;
```

### **Namespace**
Vérifier que le namespace correspond à la structure:
```csharp
namespace netIPAM.Components.Pages.Subnets  // ✅ Correct
{
    public partial class Edit { }
}
```

---

## 📦 Fichiers Générés (Prêts à l'Emploi)

Les fichiers `.razor.cs` refactorisés sont disponibles dans:
- `/home/claude/REFACTORED_FILES/Subnets/` (Phase 1)

---

## 🚀 Prochaines Étapes

1. **Valider** les fichiers refactorisés
2. **Compiler** et **tester** pour vérifier aucune régression
3. **Lancer les phases** 2 et 3 si tout fonctionne
4. **Mettre à jour** la CI/CD si nécessaire
5. **Documenter** le pattern dans les guidelines du projet

---

**Rapport généré:** 3 mai 2026  
**Analyse:** Complète  
**Fichiers à traiter:** 14 (4 priorité haute, 4 moyenne, 6 basse/optionnelle)
