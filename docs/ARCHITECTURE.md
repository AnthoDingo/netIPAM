# Architecture

## Vue d'ensemble des couches

```
┌──────────────────────────────────────────────────────────┐
│  PhpIpamNet.Web  (Blazor Server)                          │
│  - Composants Razor, Layouts, Pages                       │
│  - Authentification cookie                                │
│  - Program.cs : composition root                          │
└────────────────┬─────────────────────────────────────────┘
                 │ DI
┌────────────────▼─────────────────────────────────────────┐
│  PhpIpamNet.Infrastructure                                │
│  - PhpIpamDbContext (EF Core)                             │
│  - Services métier (Section/Subnet/Ip/User/Dashboard)     │
│  - Hashing : BCrypt + hook legacy SHA-512                 │
│  - DataSeeder                                             │
└────────────────┬─────────────────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────────────────┐
│  PhpIpamNet.Domain (aucune dépendance)                    │
│  - Entities (mappées 1-1 aux tables phpIPAM)              │
│  - Enums (IpVersion, IpAddressState)                      │
│  - Services purs (IpConverter, SubnetCalculator)          │
└──────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────┐
│  Migrations.SqlServer  /  Migrations.Sqlite               │
│  Projets séparés, références à Infrastructure pour le     │
│  DbContext. Chacun a son propre ModelSnapshot.            │
└──────────────────────────────────────────────────────────┘
```

## Décisions techniques notables

### 1. Stockage IP fidèle à phpIPAM

phpIPAM stocke les adresses IP comme **entiers décimaux** dans des colonnes texte :

- IPv4 → entier 32-bit non signé (`168427779` = `10.10.1.3`)
- IPv6 → entier 128-bit (`BigInteger`)

Cette convention est conservée. Les conversions sont centralisées dans
`PhpIpamNet.Domain.Services.IpConverter` (port des fonctions PHP
`Transform_to_decimal` / `Transform_to_dotted`).

**Pourquoi pas un type SQL Server `varbinary(16)` ou `inet`** ?
Pour permettre à un dev de pointer cette app vers une base phpIPAM existante
sans transformation.

### 2. Deux projets de migrations

EF Core génère des types SQL différents selon le provider (`TIMESTAMP` vs `INTEGER`,
`bit` vs `INTEGER`, `nvarchar(max)` vs `TEXT`, identité auto-incrément différente…).
Un projet de migrations unique partagé entre providers est fragile : le
`ModelSnapshot` ne peut pas représenter deux dialectes.

D'où la séparation :
- `PhpIpamNet.Migrations.SqlServer` : assemble pour SQL Server
- `PhpIpamNet.Migrations.Sqlite` : assemble pour SQLite

Le DbContext est unique. La sélection de provider se fait dans
`Infrastructure.DependencyInjection.AddPhpIpamPersistence` selon
`Database:Provider` (config), avec `MigrationsAssembly` ciblé en conséquence.

### 3. Mots de passe — BCrypt + hook legacy

phpIPAM utilise `crypt() SHA-512` (`$6$rounds=3000$...`). .NET ne fournit pas
`crypt()` en natif. Choix :

- **Nouveaux comptes** : BCrypt (workFactor 11) — solide et bien supporté.
- **Comptes hérités** : `LegacyCryptVerifier` est un stub avec trois stratégies
  documentées (CryptSharp NuGet, P/Invoke libcrypt, rehash-on-login).

Cette séparation rend explicite la dépendance, plutôt que de la masquer derrière
un wrapper magique qui pourrait silencieusement échouer.

### 4. Auth via cookies, pas via Identity

ASP.NET Core Identity est puissant mais opinionné — il impose son propre schéma
de tables (`AspNetUsers`…). Comme on veut rester compatible avec le schéma
phpIPAM (table `users` à colonnes spécifiques), Identity n'est pas adapté.

À la place :
- Authentification cookie pure (`AddCookie`)
- Validation du mot de passe via `UserService.AuthenticateLocalAsync`
- Création des claims manuellement après authentification réussie

Pour ajouter LDAP, SAML, OIDC, ajoutez des `AuthenticationHandler` dédiés
sans toucher au stockage des comptes locaux.

### 5. Blazor Server (et pas Blazor WebAssembly)

phpIPAM est une app PHP server-rendered traditionnelle. Blazor Server :
- Préserve le modèle "tout côté serveur"
- Pas de JS à écrire pour la plupart des interactions
- Connection unique au backend (DB, services)
- Performance acceptable pour une app admin interne

Blazor WASM aurait nécessité une couche API REST séparée (que phpIPAM a, mais
qui sort du scope de cette fondation).

## Mapping phpIPAM → C#

| phpIPAM (PHP)                              | PhpIpamNet (C#)                                       |
| ------------------------------------------ | ----------------------------------------------------- |
| `class.Subnets.php`                        | `PhpIpamNet.Domain.Services.SubnetCalculator`         |
| `class.Tools.php` (parties IP)             | `PhpIpamNet.Domain.Services.IpConverter`              |
| `class.User.php` (auth locale)             | `PhpIpamNet.Infrastructure.Services.UserService`      |
| `functions/classes/class.DB.php`           | `PhpIpamDbContext` + EF Core                          |
| `app/admin/sections/`                      | `Components/Pages/Sections/`                          |
| `app/subnets/`                             | `Components/Pages/Subnets/`                           |
| `app/dashboard/widgets/statistics.php`     | `DashboardService` + `Pages/Dashboard.razor`          |
| `app/login/index.php` + sessions PHP       | `Pages/Auth/Login.razor` + cookies ASP.NET Core       |

## Ce qui n'est volontairement pas porté

- **Custom fields dynamiques** : phpIPAM permet d'ajouter des colonnes via l'UI.
  Implémentation possible avec EF Core + table d'attributs, mais c'est un design
  à part entière.
- **Permissions par section/subnet** : encodées en JSON `{groupId:level}`.
  Lecture simple possible, mise en œuvre fine via un `AuthorizationHandler`.
- **Hooks** : phpIPAM a un système de hooks PHP. Équivalent C# = MediatR ou
  événements de domaine — à brancher si nécessaire.
- **Importation MySQL → SQL Server/SQLite** : ETL à écrire séparément (les
  conventions IP/decimal sont conservées, donc faisable colonne à colonne).

## Tests

Volontairement absents de cette fondation pour ne pas gonfler la livraison.
Recommandation :
- Tests unitaires de `IpConverter` et `SubnetCalculator` (edges : /31, /32, IPv6).
- Tests d'intégration avec SQLite in-memory (`Microsoft.EntityFrameworkCore.Sqlite` +
  `:memory:`) pour les services métier.
- Tests bUnit pour les composants Razor non triviaux (Edit, IpEdit).
