# PhpIpamNet

Port partiel de [phpIPAM](https://github.com/phpipam/phpipam) (PHP) vers **C# / .NET 10 / Blazor Server**.

> Ce dépôt est une **fondation progressive**. Chaque itération ajoute un bloc fonctionnel
> complet. Voir [Périmètre](#périmètre) pour ce qui reste à faire.

## Itérations livrées

| Itération | Contenu |
| --------- | ------- |
| **v1 — Foundation** | Schéma 19 tables, migrations EF, Sections CRUD, Subnets CRUD, IPs CRUD, auth locale, IP Calculator, Dashboard |
| **v2 — Admin** | Rôles (Admin/Operator/Guest), 14 modules admin : Users + Groups + AuthMethods + Settings + Logs + VLANs + L2 domains + VRFs + Devices + DeviceTypes + Nameservers + Customers + Locations + IpTags |

## Stack

| Couche             | Choix                                                                  |
| ------------------ | ---------------------------------------------------------------------- |
| Framework          | ASP.NET Core 10, Blazor Server (rendu interactif côté serveur)         |
| ORM                | EF Core 10, providers SQL Server **et** SQLite                         |
| Authentification   | Cookie ASP.NET Core (équivalent des sessions PHP)                      |
| Hashing            | BCrypt.Net-Next pour les nouveaux comptes (hook legacy SHA-512 fourni) |
| UI                 | Bootstrap 5 + Bootstrap Icons (look phpIPAM-like)                      |

## Démarrage rapide (SQLite, par défaut)

```bash
dotnet restore
dotnet ef migrations add InitialCreate \
    -p src/PhpIpamNet.Migrations.Sqlite \
    -s src/PhpIpamNet.Web
dotnet run --project src/PhpIpamNet.Web
```

Au premier démarrage, la base `phpipam_net.db` est créée et un utilisateur **Admin / ipamadmin**
est ajouté automatiquement (à changer immédiatement). Naviguez vers `https://localhost:7180`.

## Bascule vers SQL Server

Modifiez `src/PhpIpamNet.Web/appsettings.json` :

```json
"Database": {
  "Provider": "SqlServer",
  "ConnectionString": "Server=localhost;Database=phpipam_net;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Générez la migration pour SQL Server :

```bash
dotnet ef migrations add InitialCreate \
    -p src/PhpIpamNet.Migrations.SqlServer \
    -s src/PhpIpamNet.Web
dotnet run --project src/PhpIpamNet.Web
```

> Les types SQL ne sont **pas identiques** entre les deux providers (TIMESTAMP, BOOL, BINARY,
> AUTOINCREMENT…). C'est pourquoi chaque provider a son propre projet de migrations
> (`Migrations.SqlServer`, `Migrations.Sqlite`) avec son propre `ModelSnapshot`.
> EF Core génère un snapshot adapté à chaque provider lors du `migrations add`.

## Structure de la solution

```
PhpIpamNet/
├── PhpIpamNet.sln
├── src/
│   ├── PhpIpamNet.Domain/                     Entités, enums, services purs (IpConverter, SubnetCalculator)
│   ├── PhpIpamNet.Infrastructure/             DbContext, services métier, hashing
│   ├── PhpIpamNet.Migrations.SqlServer/       Migrations spécifiques SQL Server
│   ├── PhpIpamNet.Migrations.Sqlite/          Migrations spécifiques SQLite
│   └── PhpIpamNet.Web/                        Blazor Server, pages Razor, layout
└── docs/
    ├── ARCHITECTURE.md
    └── reference-InitialCreate-SqlServer.cs.txt   Migration SQL Server pré-écrite (référence)
```

## Périmètre

### Inclus (v1 + v2)

**v1 — Foundation**
- Schéma de base : 19 tables fidèles à phpIPAM
- Conventions de stockage IP (entier décimal en string, IPv4/IPv6)
- Conversion IP, calcul de sous-réseau (port de `class.Subnets.php`)
- CRUD Sections, Subnets (avec hiérarchie + folders), IP addresses, Users
- Calculateur IPv4/IPv6, Dashboard avec statistiques
- Authentification locale + cookies ASP.NET Core

**v2 — Administration complète**
- Système de rôles : `Administrator`, `Operator`, `Guest`
  - Policies `Admin` / `Operator` sur toutes les routes concernées
  - NavMenu filtré selon le rôle via `<AuthorizeView Policy="...">`
- **Système** (Admin uniquement) :
  - Users CRUD + reset mot de passe, guard "dernier Administrator"
  - User groups CRUD
  - Auth methods CRUD (local/http + placeholders LDAP/AD/SAML/Radius)
  - Settings singleton (modules on/off, site identity, auth timeouts)
  - Logs read-only avec filtre par sévérité
- **Réseau** (Operator+) :
  - VLANs CRUD avec sélecteur de L2 domain
  - L2 domains CRUD, guard domaine 1 (default)
  - VRFs CRUD avec Route Distinguisher
  - Devices CRUD avec SNMP + location + type
  - Device types CRUD
  - Nameservers CRUD (multi-adresses séparées par ';')
- **Métier** (Operator+) :
  - Customers CRUD (adresse complète, contact, statut)
  - Locations CRUD (coordonnées géographiques)
  - IP tags CRUD (couleurs, guard tags système Locked)

### À itérer (hors périmètre actuel)

| Module | Priorité suggérée |
| ------ | ----------------- |
| LDAP / AD / SAML / Radius | Haute si Active Directory en prod |
| API REST | Haute pour automatisation |
| Permissions fines par section/subnet (JSON `{"groupId":"level"}`) | Moyenne |
| Changelog UI (table déjà présente, écriture manquante) | Basse |
| Scan / discovery (Ping, SNMP) | Basse (dépendances système) |
| NAT, Racks, PSTN, Circuits | Verticaux indépendants |
| Custom fields | Métaprogrammation EF Core |
| Multilingue | Resx + gettext phpIPAM |
| 2FA / passkeys | Module identité séparé |
| Migration depuis MySQL | ETL à écrire |

## Compatibilité avec une base phpIPAM existante

Les noms de tables et de colonnes sont fidèles au schéma phpIPAM. Le mapping
`[Table]/[Column]` dans `PhpIpamNet.Domain.Entities` garantit que pointer cette app
vers une base phpIPAM existante doit fonctionner pour les modèles couverts.

**Limite importante** : phpIPAM stocke les mots de passe en `crypt() SHA-512`
(`$6$rounds=3000$...`). .NET ne fournit pas `crypt()` en natif. Le fichier
`Infrastructure/Identity/LegacyCryptVerifier.cs` est un stub qui retourne `false` —
documenté avec trois options de mise en œuvre :

1. Ajouter le NuGet `CryptSharp` et appeler `Crypter.CheckPassword`.
2. P/Invoke vers `libcrypt` sous Linux.
3. Stratégie de **rehash-on-login** : sur la première connexion réussie en SHA-512,
   re-hasher en BCrypt et persister, ce qui éteint progressivement la dépendance.

Tant que ce stub n'est pas branché, les comptes hérités ne peuvent pas se connecter
par mot de passe — c'est volontairement explicite.

## Commandes EF Core utiles

```bash
# Créer une nouvelle migration
dotnet ef migrations add <Name> -p src/PhpIpamNet.Migrations.SqlServer -s src/PhpIpamNet.Web
dotnet ef migrations add <Name> -p src/PhpIpamNet.Migrations.Sqlite    -s src/PhpIpamNet.Web

# Appliquer les migrations à la base (utile en production sans .MigrateAsync au démarrage)
dotnet ef database update -p src/PhpIpamNet.Migrations.SqlServer -s src/PhpIpamNet.Web
dotnet ef database update -p src/PhpIpamNet.Migrations.Sqlite    -s src/PhpIpamNet.Web

# Générer un script SQL idempotent
dotnet ef migrations script -p src/PhpIpamNet.Migrations.SqlServer -s src/PhpIpamNet.Web -o init.sql --idempotent
```

Pour cibler le bon provider, `Database:Provider` doit être positionné dans la config
ou via la variable d'environnement `ASPNETCORE_Database__Provider`.

## Licence

Le code de cette fondation est fourni tel quel. Vérifiez la licence du projet phpIPAM
original (GPL v3) pour les contraintes liées à l'inspiration du schéma et de l'UI.
