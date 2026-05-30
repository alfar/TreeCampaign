# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**TreeCampaign** is a full-stack management app for a yearly scout Christmas tree collection event in the 8600 postal area (Silkeborg, Denmark). Citizens submit a payment (40 DKK/tree) along with their address; scout teams are dispatched over multiple days to collect trees. The system manages the full pipeline from raw payment intake through address validation, stop assignment, and collection tracking.

The developer is learning Domain-Driven Design by building this system. Claude's role is to coach, not to implement. Preserve this dynamic — offer guidance, ask clarifying questions about design decisions, and let the developer write the code.

## Bounded Contexts

The solution contains three bounded contexts, all within one .NET solution. They are independent by design — no project may reference across context boundaries except through defined interfaces or events.

### TreeCampaign (core domain) — implemented

Manages campaign seasons, stop assignment, and team dispatch. Receives validated orders from Intake and creates stops.

**Stop ordering** is a read-model concern only: when displaying a team's assigned stops, sort by `StreetSection.SortOrder` (from Territory), then by house number ascending or descending per `StreetSection.Direction`. The old paper route cap of 8 trees is gone — the dispatcher assigns stops to teams dynamically in real time.

### Territory (supporting subdomain) — implemented

The authority on which addresses exist in the service area and how they are traversed. Has no knowledge of payments, stops, or campaigns.

**Domain model:**
```
Territory (aggregate root)
  └─ Neighborhood (aggregate root, child of Territory)
        └─ StreetSection (entity, child of Neighborhood)
              ├─ Street reference (by StreetId — Street is a separate aggregate)
              ├─ HouseNumberFrom / HouseNumberTo
              ├─ Direction (enum: Ascending | Descending)   ← direction of travel within the section
              └─ SortOrder (int)        ← position in the neighborhood's route

Street (separate aggregate — a street can span multiple neighborhoods)
  ├─ Name
  └─ ZipCode
```

**Aggregate Boundaries:**
- `Neighborhood` is the aggregate root; `StreetSection` can only be created through `Neighborhood.AddStreetSection()`. The internal `StreetSection.Create()` factory is not publicly accessible.
- `Street` is a separate aggregate referenced by StreetId (no navigation property to Street itself—Territory has no knowledge of Street internals).
- The relation is `Territory → Neighborhood → StreetSection ← Street` (by reference).

**Implementation notes:**
- ID generation: All aggregates auto-generate GUIDs in their factory methods (e.g., `Territory.Create(name)` generates its own TerritoryId).
- `Neighborhood._streetSections` is a backing field with EF Core field access mode; the public `StreetSections` property is read-only.
- Direction enum stored as `byte` in SQLite for space efficiency.
- Repository pattern: TreeTerritoryContext implements `IUnitOfWork` and `IRepository<T, TId>` for all aggregates.

**Responsibilities:** validate a raw address against known streets and number ranges; return structured address data and sort metadata to callers; accept "add new street" commands triggered by Intake.

### Intake (supporting subdomain) — not yet built

Turns raw CSV payment rows into validated orders submitted to TreeCampaign.

**Domain model:**
```
IncomingOrder
  ├─ RawText       (original CSV line, including any greeting text)
  ├─ Amount (DKK)
  ├─ WashedAddress (nullable — set after address washing)
  └─ State: Pending | AwaitingResolution | Validated | Submitted | Rejected
```

**Flow:**
1. Import CSV → one `IncomingOrder` per row, state `Pending`
2. Wash address text (strip greetings, extract the address string)
3. Validate against Territory → match found: `Validated`; no match: `AwaitingResolution`
4. Resolution queue (human reviews unresolved orders), three outcomes:
   - **Reject** → `Rejected`, trigger refund
   - **Adjust** → correct address, re-validate against Territory
   - **Add street** → instruct Territory to add the street, then re-validate
5. `Validated` → submit to TreeCampaign, which creates an `UnassignedStop`

`AddressResolution` is an Intake concern. Territory is a service Intake calls; the pending payment lifecycle belongs to Intake.

## Context Interactions

```
Intake ──validates──► Territory       ("is this address in our service area?")
Intake ──add street─► Territory       ("add this previously unknown street")
Intake ──submits────► TreeCampaign    ("create a stop for this validated order")
TreeCampaign ──reads► Territory       (projection: sort order + direction for stop display)
```

## Open Design Questions

These are unsettled and should be discussed before building the relevant parts:

- How do Intake and TreeCampaign communicate when an order is submitted — domain event, direct call, or anti-corruption layer?
- Address washing: regex heuristic or LLM-assisted?

## Build & Run Commands

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

### Entity Framework Migrations
```powershell
# TreeCampaign migrations (TreeCampaign.Repository)
dotnet ef database update --project TreeCampaign.Repository --startup-project Host.Api
dotnet ef migrations add <Name> --project TreeCampaign.Repository --startup-project Host.Api

# Territory migrations (TreeTerritory.Repository)
dotnet ef database update --project TreeTerritory.Repository --startup-project Host.Api
dotnet ef migrations add <Name> --project TreeTerritory.Repository --startup-project Host.Api
```

Database is SQLite, written to `{BaseDirectory}/app.db` at runtime.

## Project Structure

| Project | Type | Role |
|---|---|---|
| `Common.Repository` | Class Library | Shared abstractions: `IUnitOfWork`, `IRepository<TAggregate, TId>` |
| `TreeCampaign.Domain` | Class Library | Pure domain logic — no external dependencies |
| `TreeCampaign.Repository` | Class Library | EF Core + SQLite, dual DbContext pattern |
| `TreeCampaign.Api` | Class Library | Endpoint extension methods for TreeCampaign context |
| `TreeTerritory.Domain` | Class Library | Pure domain logic for Territory context |
| `TreeTerritory.Repository` | Class Library | EF Core persistence for Territory context |
| `TreeTerritory.Api` | Class Library | Endpoint extension methods for Territory context |
| `Host.Api` | ASP.NET Core | Web host — wires up all bounded context endpoints |
| `TreeCampaign.UI` | React/Vite | Frontend SPA |

Dependency direction: `Host.Api` → `*.Api` → `*.Repository` → `*.Domain`.

### Domain Model (Stop state machine)

Stops use **type-based state** (Zoran Horvat style) — no `State` property, no enums. Each class exposes only the behavior valid in that state; transitions return a new instance.

```
UnassignedStop → AssignedStop → CollectedStop
                      ↓               ↓
                 (unassign)     MarkUnresolved → UnresolvedStop
                                                     ↓
                                               Retry → AssignedStop
                                               Reopen → UnassignedStop

AssignedStop ← CorrectMistakenCollection ← CollectedStop
```

Each transition raises an `IDomainEvent`. Events are persisted to `StoredDomainEvents` and cleared from the aggregate after save, intended for future projections and analytics.

EF Core maps the hierarchy via **Table-Per-Hierarchy (TPH)** with a `StopType` discriminator column. The repository explicitly nulls `AssignedTeamId` when saving an `UnassignedStop` — an invariant EF cannot enforce through TPH alone.

### Data Access

- **`TreeCampaignContext`** — write model (aggregates + event log)
- **`ProjectionContext`** — read-only, `AsNoTracking`, flat DTOs; never exposes domain entities to the UI
- Repositories via `IUnitOfWork.GetRepository<TAggregate, TId>()`
- All Value Objects have EF Core value converters (e.g., `StopId` ↔ `Guid`)

### API

Minimal API, no controllers. Endpoints in extension methods: `MapCampaignEndpoints()`, `MapStopEndpoints()`, `MapTeamEndpoints()`. All routes prefixed `/api`. Action-based, no PATCH. Custom JSON converters for Value Objects. Scalar UI for exploration.

Key stop action endpoints (all `POST`):
`/api/{campaignId}/stops/{stopId}/assign|unassign|collect|unresolved|reopen|correct|retry`

### Frontend

Layered: `client.ts` → hooks → screens → components. React Router v7.

- `/` → Campaign list
- `/campaigns/:campaignId/dispatch` → Dispatch screen (assign stops to teams)
- `/campaigns/:campaignId/teams/:teamId` → Team detail view

Vite proxies `/api/*` to `:5006`.

**Dispatch UI principle**: fast decisions under mild pressure. Stops grouped by street/area; dispatcher selects group, selects team. No map required.

## Technology Stack

- **.NET 10** with C# 13
- **EF Core 10.0.7** with SQLite
- **Value Objects:** Configure all value object properties with either:
  1. **ValueConverter** (traditional): `builder.Property(s => s.ZipCode).HasConversion(new ZipCodeValueConverter())`
  2. **ComplexProperty** (EF Core 8+): `builder.ComplexProperty(s => s.Address, a => { a.Property(p => p.DisplayName).HasColumnName("AddressDisplayName"); })`
  
  The error "entity type requires a primary key" occurs when a value object property is discovered by EF Core's model builder but lacks configuration. Always add a converter or mark it as a complex property. Both approaches work; choose based on whether the VO is simple (converter) or composite (complex property).

## Key Conventions

- **Value Objects** as C# records — prefer them over primitives in domain code.
- **Sealed classes** for Stop state variants — use pattern matching. No enums for state.
- DI registration centralized in `ServiceExtensions.AddTreeCampaign()`.
- No dedicated test projects — Scalar UI serves for integration validation.
- Favor practical usability over theoretical optimality; explicit modeling over clever abstraction.
