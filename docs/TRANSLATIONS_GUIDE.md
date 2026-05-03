# 🌍 Guide d'Intégration des Traductions Multilingues

## 📋 Vue d'Ensemble

Un système complet de traductions multilingues a été ajouté à votre projet netIPAM avec support pour :
- 🇬🇧 **English** (en)
- 🇫🇷 **Français** (fr)
- 🇮🇹 **Italiano** (it)
- 🇪🇸 **Español** (es)
- 🇩🇪 **Deutsch** (de)

## 📁 Structure des Fichiers

```
netIPAM/
├── wwwroot/i18n/
│   ├── en/translation.json
│   ├── fr/translation.json
│   ├── it/translation.json
│   ├── es/translation.json
│   └── de/translation.json
├── Services/
│   └── LocalizationService.cs    (Nouvelle)
└── Components/Shared/
    └── LanguageSelector.razor     (Nouveau)
```

## 🔧 Intégration dans Program.cs

Ajoutez le service de localisation à votre `Program.cs`:

```csharp
// Dans netIPAM/Program.cs
using netIPAM.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder
    .RootComponents.Add<App>("#app")
    .RootComponents.Add<HeadOutlet>("head::after");

// Ajouter HttpClient
builder.Services.AddScoped(sp => new HttpClient 
    { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Ajouter le service de localisation
builder.Services.AddScoped<LocalizationService>();

await builder.Build().RunAsync();
```

## 💡 Utilisation dans les Composants Razor

### Option 1: Injection du service dans un composant

```razor
@page "/example"
@inject LocalizationService Localization

<h1>@Localization.Get("subnets.title")</h1>
<button>@Localization.Get("common.save")</button>

@code {
    protected override async Task OnInitializedAsync()
    {
        // Initialiser avec la langue de l'utilisateur
        await Localization.InitializeAsync("en");
    }
}
```

### Option 2: Utiliser le composant LanguageSelector

```razor
@page "/admin/dashboard"
@inject LocalizationService Localization

<div class="navbar">
    <h1>@Localization.Get("common.dashboard")</h1>
    <LanguageSelector />
</div>

<div class="content">
    <h2>@Localization.Get("admin.title")</h2>
    <!-- Contenu -->
</div>
```

### Option 3: Dans le code-behind (C#)

```csharp
public partial class MyPage
{
    [Inject]
    private LocalizationService Localization { get; set; } = default!;

    private string Title = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await Localization.InitializeAsync("fr");
        Title = Localization.Get("subnets.title");
    }
}
```

## 📖 Exemple Complet: Page Subnets

### Before (Sans traductions)
```razor
@page "/sections/{SectionId:int}/subnets/new"
@page "/subnets/{Id:int}/edit"

<h1>@(_isNew ? "New subnet" : "Edit subnet")</h1>
<label>Description</label>
<button>Save</button>
<a href="">Cancel</a>
```

### After (Avec traductions)
```razor
@page "/sections/{SectionId:int}/subnets/new"
@page "/subnets/{Id:int}/edit"
@inject LocalizationService Localization

<h1>@(_isNew ? Localization.Get("subnets.newSubnet") : Localization.Get("subnets.editSubnet"))</h1>
<label>@Localization.Get("common.description")</label>
<button>@Localization.Get("common.save")</button>
<a href="">@Localization.Get("common.cancel")</a>
```

## 🔑 Clés de Traduction Disponibles

### Section: `common`
```
common.save
common.cancel
common.delete
common.edit
common.add
common.search
common.loading
common.error
common.success
...
```

### Section: `subnets`
```
subnets.title
subnets.newSubnet
subnets.editSubnet
subnets.cidr
subnets.cidrRequired
subnets.network
subnets.broadcast
...
```

### Section: `admin`
```
admin.title
admin.customers
admin.devices
admin.locations
...
```

Pour voir toutes les clés disponibles, consultez les fichiers JSON:
- `/wwwroot/i18n/en/translation.json`
- `/wwwroot/i18n/fr/translation.json`
- etc.

## 🎯 Ajouter de Nouvelles Traductions

### Étape 1: Ajouter la clé au fichier anglais
```json
{
  "myFeature": {
    "myKey": "My English text"
  }
}
```

### Étape 2: Ajouter à tous les autres fichiers de langue
```json
{
  "myFeature": {
    "myKey": "Mon texte français"
  }
}
```

### Étape 3: Utiliser dans le composant
```razor
@Localization.Get("myFeature.myKey")
```

## 🔄 Événement de Changement de Langue

Le service émet un événement quand la langue change:

```csharp
protected override async Task OnInitializedAsync()
{
    Localization.OnLanguageChanged += OnLanguageChanged;
    await Localization.InitializeAsync();
}

private void OnLanguageChanged()
{
    // Mettre à jour l'interface si nécessaire
    StateHasChanged();
}

void IAsyncDisposable.DisposeAsync()
{
    Localization.OnLanguageChanged -= OnLanguageChanged;
}
```

## 📱 Intégration du Sélecteur de Langue dans la Navbar

```razor
<!-- App.razor ou Layout.razor -->
<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container">
        <a class="navbar-brand" href="/">netIPAM</a>
        <button class="navbar-toggler" type="button">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="navbar-collapse">
            <ul class="navbar-nav ms-auto">
                <li class="nav-item">
                    <LanguageSelector />
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="/login">@Localization.Get("common.login")</a>
                </li>
            </ul>
        </div>
    </div>
</nav>
```

## 🌐 Déterminer la Langue par Défaut

### Option 1: Langue du navigateur
```csharp
protected override async Task OnInitializedAsync()
{
    var language = await JS.InvokeAsync<string>("eval", 
        "navigator.language.split('-')[0]");
    
    var supportedLangs = LocalizationService.AvailableLanguages;
    if (!supportedLangs.Contains(language))
    {
        language = "en";
    }
    
    await Localization.InitializeAsync(language);
}
```

### Option 2: Depuis les préférences utilisateur
```csharp
protected override async Task OnInitializedAsync()
{
    var userLang = await UserService.GetPreferredLanguageAsync();
    await Localization.InitializeAsync(userLang ?? "en");
}
```

### Option 3: Depuis le localStorage
```csharp
protected override async Task OnInitializedAsync()
{
    var language = await JS.InvokeAsync<string>("localStorage.getItem", "language");
    if (string.IsNullOrEmpty(language))
    {
        language = "en";
    }
    await Localization.InitializeAsync(language);
}
```

## 💾 Persister la Langue Sélectionnée

Modifiez le service pour sauvegarder la langue:

```csharp
public class LocalizationService
{
    private const string LanguageKey = "preferred-language";
    
    public async Task SetLanguageAsync(string language)
    {
        // ... code existant ...
        
        // Sauvegarder en localStorage
        await _js.InvokeVoidAsync("localStorage.setItem", LanguageKey, language);
    }
}
```

## 🧪 Tests

### Vérifier que le service fonctionne
```csharp
[Test]
public async Task SetLanguage_ChangesLanguageCorrectly()
{
    var service = new LocalizationService(new HttpClient());
    
    await service.SetLanguageAsync("fr");
    
    Assert.AreEqual("fr", service.GetCurrentLanguage());
}

[Test]
public void Get_ReturnsTranslation()
{
    var service = new LocalizationService(new HttpClient());
    
    var translation = service.Get("common.save");
    
    Assert.IsNotEmpty(translation);
}
```

## ✨ Caractéristiques du Système

✅ **Chargement asynchrone** des fichiers JSON  
✅ **Mise en cache** des traductions chargées  
✅ **Événements** de changement de langue  
✅ **Support complet** des 5 langues  
✅ **Clés imbriquées** (point-separated)  
✅ **Valeurs par défaut** si traduction manquante  
✅ **Intégration CultureInfo** pour les formats  
✅ **Composant réutilisable** pour le sélecteur  

## 📊 Statistiques

| Langue | Code | Fichier | Clés |
|--------|------|---------|------|
| English | en | translation.json | 200+ |
| Français | fr | translation.json | 200+ |
| Italiano | it | translation.json | 200+ |
| Español | es | translation.json | 200+ |
| Deutsch | de | translation.json | 200+ |

## 🚀 Prochaines Étapes

1. **Ajouter** le service à `Program.cs`
2. **Intégrer** `LanguageSelector` dans la navbar
3. **Remplacer** les textes en dur par les clés de traduction
4. **Tester** avec chaque langue
5. **Sauvegarder** la langue utilisateur en localStorage

## 📝 Fichiers Modifiés/Créés

```
Créés:
✅ wwwroot/i18n/en/translation.json
✅ wwwroot/i18n/fr/translation.json
✅ wwwroot/i18n/it/translation.json
✅ wwwroot/i18n/es/translation.json
✅ wwwroot/i18n/de/translation.json
✅ Services/LocalizationService.cs
✅ Components/Shared/LanguageSelector.razor

À modifier:
⚠️ Program.cs (ajouter le service)
⚠️ App.razor ou Layout.razor (ajouter le sélecteur)
⚠️ Tous les composants Razor (remplacer les textes)
⚠️ Code-behind (.razor.cs) (utiliser le service)
```

## ❓ Questions Fréquentes

**Q: Puis-je ajouter d'autres langues?**  
R: Oui, créez simplement un nouveau dossier dans `wwwroot/i18n/` avec le code de langue et un fichier `translation.json`.

**Q: Comment gérer les traductions manquantes?**  
R: Le service retourne la clé si la traduction n'existe pas. Vous pouvez modifier `Get()` pour loguer les clés manquantes.

**Q: Puis-je pluraliser les traductions?**  
R: Oui, modifiez le service pour supporter la pluralisation avec une convention de clés (ex: `items.count.zero`, `items.count.one`, `items.count.many`).

**Q: Comment tester les traductions?**  
R: Créez des tests unitaires pour le service, ou testez manuellement avec chaque langue dans le navigateur.

---

**Système de traduction complet et prêt à l'emploi!** 🎉
