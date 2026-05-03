# netIPAM

Port .NET 10 / Blazor Server de [phpIPAM](https://github.com/phpipam/phpipam) — projet unique, zéro dépendance inter-projets.

## Structure

```
netIPAM/
├── netIPAM.csproj          SDK Web — tous les packages
├── Program.cs              Composition root
├── Data/                   DbContext (AppDbContext)
├── Entities/               20 entités EF Core (fidèles au schéma phpIPAM)
├── Enums/                  IpVersion, IpAddressState
├── Services/               21 services métier + DI + DataSeeder
├── Identity/               BCrypt + hook legacy SHA-512
├── Migrations/
│   ├── SqlServer/          Factory design-time SqlServer
│   └── Sqlite/             Factory design-time Sqlite
├── Components/             Blazor — App, Routes, Layout, Pages, Shared
└── wwwroot/                CSS, JS
```

## Démarrage rapide (SQLite)

```bash
dotnet ef migrations add InitialCreate \
    -o Migrations/Sqlite \
    --namespace netIPAM.Migrations.Sqlite

dotnet run
```
→ `https://localhost:7180` — Admin / ipamadmin

## Bascule SQL Server

Dans `appsettings.json` :
```json
"Database": { "Provider": "SqlServer", "ConnectionString": "Server=...;Database=netIPAM;..." }
```

Puis :
```bash
dotnet ef migrations add InitialCreate \
    -o Migrations/SqlServer \
    --namespace netIPAM.Migrations.SqlServer
dotnet run
```

## Commandes EF utiles

```bash
# Sqlite
dotnet ef migrations add <Nom> -o Migrations/Sqlite --namespace netIPAM.Migrations.Sqlite
dotnet ef database update

# SqlServer
dotnet ef migrations add <Nom> -o Migrations/SqlServer --namespace netIPAM.Migrations.SqlServer
dotnet ef database update
```
