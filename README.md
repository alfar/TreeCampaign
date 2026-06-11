# TreeCampaign

Full-stack management app for a yearly scout Christmas tree collection event in the 8600 postal area (Silkeborg, Denmark).

## Build & Run

### Backend (.NET)
```powershell
dotnet build TreeCampaign.sln
dotnet run --project Host.Api          # API on port 5006
```

### Frontend
```powershell
cd TreeCampaign.UI
npm install
npm run dev      # Dev server at http://localhost:5173 (proxies /api → :5006)
npm run build
npm run lint
```

## Entity Framework Migrations

Database is SQLite, written to `{BaseDirectory}/app.db` at runtime.

```powershell
# StoredDomainEvents (Common.Infrastructure — owns the StoredDomainEvents table)
dotnet ef migrations add <Name> --project Common.Infrastructure --startup-project Host.Api --context StoredDomainEventContext
dotnet ef database update --project Common.Infrastructure --startup-project Host.Api --context StoredDomainEventContext

# TreeCampaign migrations (TreeCampaign.Infrastructure)
dotnet ef migrations add <Name> --project TreeCampaign.Infrastructure --startup-project Host.Api --context TreeCampaignContext
dotnet ef database update --project TreeCampaign.Infrastructure --startup-project Host.Api --context TreeCampaignContext

# Territory migrations (TreeTerritory.Infrastructure)
dotnet ef migrations add <Name> --project TreeTerritory.Infrastructure --startup-project Host.Api --context TreeTerritoryContext
dotnet ef database update --project TreeTerritory.Infrastructure --startup-project Host.Api --context TreeTerritoryContext

# Intake migrations (Intake.Infrastructure)
dotnet ef migrations add <Name> --project Intake.Infrastructure --startup-project Host.Api --context IntakeContext
dotnet ef database update --project Intake.Infrastructure --startup-project Host.Api --context IntakeContext
```
