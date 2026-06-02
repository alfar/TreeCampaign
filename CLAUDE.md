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

**Campaign** carries a `TerritoryRef` (local value object wrapping a Guid, defined in `TreeCampaign.Domain/Campaigns/ExternalReferences/`) to scope address validation to the right territory. This is a correlation ID — Campaign has no compile-time dependency on the Territory domain.

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
- Repository pattern: `TreeTerritoryContext` implements `ITreeTerritoryUnitOfWork` and `IRepository<T, TId>` for all aggregates.

**Responsibilities:** validate a raw address against known streets and number ranges; return structured address data and sort metadata to callers; accept "add new street" commands triggered by Intake.

### Intake (supporting subdomain) — domain layer implemented

Turns raw payment messages into validated orders submitted to TreeCampaign.

**Domain model — type-based state machine** (same Zoran Horvat style as Stops):

```
IncomingOrder
  ├─ Accept(ValidationSuccess)       → ValidatedOrder
  ├─ MarkUnwashed()                  → UnwashedOrder      (street not found, or parse failed)
  └─ MarkOutOfBounds(result)         → OutOfBoundsOrder   (street found, house number outside sections)

UnwashedOrder
  ├─ Accept(ValidationSuccess)       → ValidatedOrder     (bulk retry: street now exists in Territory)
  └─ Wash(WashedAddress)             → WashedOrder

WashedOrder
  ├─ Accept(ValidationSuccess)       → ValidatedOrder
  └─ MarkOutOfBounds(result)         → OutOfBoundsOrder

OutOfBoundsOrder
  └─ Accept(ValidationSuccess)       → ValidatedOrder     (retry after Territory section expanded)
```

`OrderBase` carries: `OrderId`, `CampaignRef`, `Sender` (name + phone), `MoneyAmount`, `OrderDate`, `Message` (original free text).

**Value objects:**
- `ParsedAddress(StreetName, HouseNumber, ZipCode?, City?)` — what the auto-parser extracted from `Message`
- `WashedAddress(StreetName, HouseNumber, ZipCode?)` — what a human operator explicitly supplied
- Cross-context refs in `ExternalReferences/`: `TerritoryRef`, `NeighborhoodRef`, `StreetRef`, `StreetSectionRef`, `CampaignRef`

**Service interfaces (in `Intake.Domain/Orders/Services/`):**
- `IAddressParser` — `ParsedAddress? TryParse(string message)`
- `IAddressValidationService` — `Task<AddressValidationResult> ValidateAsync(ParsedAddress, CampaignRef, CancellationToken)`
- `AddressValidationResult` — sealed discriminated union: `ValidationSuccess` | `StreetNotFound` | `HouseNumberOutOfBounds`

**What's not yet built:** `Intake.Repository` (EF Core, validation service implementation), `Intake.Api` (endpoints).

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
- Address parsing: regex heuristic or LLM-assisted?
- Should `OutOfBoundsOrder` be re-validatable directly, or does it always require human review first?

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

# Intake migrations (Intake.Repository) — not yet created
dotnet ef database update --project Intake.Repository --startup-project Host.Api
dotnet ef migrations add <Name> --project Intake.Repository --startup-project Host.Api
```

Database is SQLite, written to `{BaseDirectory}/app.db` at runtime.

## Project Structure

| Project | Type | Role |
|---|---|---|
| `Common.Domain` | Class Library | Shared domain abstractions: `IDomainEvent` |
| `Common.Repository` | Class Library | Shared infrastructure abstractions: `IUnitOfWork`, `IRepository<TAggregate, TId>` |
| `TreeCampaign.Domain` | Class Library | Pure domain logic — no external dependencies |
| `TreeCampaign.Repository` | Class Library | EF Core + SQLite, dual DbContext pattern |
| `TreeCampaign.Api` | Class Library | Endpoint extension methods for TreeCampaign context |
| `TreeTerritory.Domain` | Class Library | Pure domain logic for Territory context |
| `TreeTerritory.Repository` | Class Library | EF Core persistence for Territory context |
| `TreeTerritory.Api` | Class Library | Endpoint extension methods for Territory context |
| `Intake.Domain` | Class Library | Pure domain logic for Intake context |
| `Host.Api` | ASP.NET Core | Web host — wires up all bounded context endpoints |
| `TreeCampaign.UI` | React/Vite | Frontend SPA |

Dependency direction: `Host.Api` → `*.Api` → `*.Repository` → `*.Domain` → `Common.Domain`.

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

Each transition raises an `IDomainEvent` (from `Common.Domain`). Events are persisted to `StoredDomainEvents` and cleared from the aggregate after save, intended for future projections and analytics.

EF Core maps the hierarchy via **Table-Per-Hierarchy (TPH)** with a `StopType` discriminator column. The repository explicitly nulls `AssignedTeamId` when saving an `UnassignedStop` — an invariant EF cannot enforce through TPH alone.

### Data Access

- **`TreeCampaignContext`** — write model (aggregates + event log); implements `ITreeCampaignUnitOfWork`
- **`ProjectionContext`** — read-only, `AsNoTracking`, flat DTOs; never exposes domain entities to the UI
- Repositories via the context-specific unit of work interface (see IUnitOfWork convention below)
- All Value Objects have EF Core value converters (e.g., `StopId` ↔ `Guid`)

### IUnitOfWork Convention

Each bounded context defines its own empty sub-interface of `IUnitOfWork` to avoid DI registration conflicts when multiple contexts are registered in one host:

```csharp
public interface ITreeCampaignUnitOfWork : IUnitOfWork { }
public interface ITreeTerritoryUnitOfWork : IUnitOfWork { }
public interface IIntakeUnitOfWork : IUnitOfWork { }
```

Each context's `ServiceExtensions` registers only its own sub-interface. Endpoints inject the context-specific interface, never the base `IUnitOfWork`.

### Cross-Context References

When one bounded context needs to reference an entity from another, it defines a local **reference record** in an `ExternalReferences/` folder — a named wrapper around a `Guid` with no compile-time dependency on the other context:

```csharp
// In Intake.Domain/Orders/ExternalReferences/
public record TerritoryRef(Guid Value) { public static TerritoryRef From(Guid v) => new(v); }
```

This keeps contexts isolated at the project level while preserving meaningful naming. The same pattern applies to `TerritoryRef` in `TreeCampaign.Domain/Campaigns/ExternalReferences/`.

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
- **Type-based state machines** for aggregates with lifecycle — no `State` enum, no `State` property. Each state is its own class exposing only valid behavior. Transitions return a new instance via `internal static CreateFrom()` factories.
- **ExternalReferences/** folder for cross-context IDs — local record wrappers around Guid, never imported types from another context's domain project.
- DI registration centralized in `ServiceExtensions.Add*()` per bounded context.
- No dedicated test projects — Scalar UI serves for integration validation.
- Favor practical usability over theoretical optimality; explicit modeling over clever abstraction.
