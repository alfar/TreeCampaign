# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**TreeCampaign** is a full-stack management app for a yearly scout Christmas tree collection event in the 8600 postal area (Silkeborg, Denmark). Citizens submit a payment (40 DKK/tree) along with their address; scout teams are dispatched over multiple days to collect trees. The system manages the full pipeline from raw payment intake through address validation, stop assignment, and collection tracking.

The developer is learning Domain-Driven Design by building this system. Claude's role is to coach first, then help with implementation when requested.

## Bounded Contexts

The solution contains four bounded contexts, all within one .NET solution. They are independent by design — no project may reference across context boundaries except through defined interfaces or events.

### Access (supporting subdomain) — implemented

Owns authentication and multi-tenancy. A `ScoutGroup` is the tenant boundary; a `User` belongs to exactly one `ScoutGroup` and authenticates via email/password (ASP.NET Core Identity's `IPasswordHasher<User>`, cookie auth scheme).

**Domain model:**
```
ScoutGroup (aggregate root) — Id, Name
User (aggregate root)
  ├─ ScoutGroupId
  ├─ Email (value object)
  ├─ PasswordHash
  └─ IsPlatformAdmin (bool — cross-tenant override, granted/revoked explicitly)
```

**Auth flow:** `POST /api/access/auth/login` verifies credentials and issues a cookie carrying claims: `NameIdentifier` (UserId), `Email`, `Name` (DisplayName), and two custom claims defined in `Common.Infrastructure/Auth/AccessClaimTypes.cs` — `scout_group_id` and `platform_admin`. `Logout` and `Me` endpoints round out the flow.

**Cross-context tenant scoping:** every other bounded context stamps its owned aggregates with a `ScoutGroupRef` (the `ExternalReferences/` pattern — see below) — currently `Territory.ScoutGroupId` and `Campaign.ScoutGroupId`. `ICurrentUserAccessor` (in `Common.Infrastructure/Auth/`) reads the claims off `HttpContext` and exposes `Guid? ScoutGroupId` / `bool IsPlatformAdmin`. Each context defines its own `*OwnershipExtensions.IsOwnedByCurrentScoutGroupAsync(id, currentUser, ct)` helper (e.g. `TreeTerritory.Api/Helpers/TerritoryOwnershipExtensions.cs`, `TreeCampaign.Api/Helpers/CampaignOwnershipExtensions.cs`) that endpoints call before allowing mutation — implemented as a projection/query lookup, not a `UnitOfWork` round-trip. `AuthPolicies.ScoutGroupMember` (in `Common.Infrastructure`) is the baseline `[Authorize]` policy applied to endpoints; ownership checks are an additional, explicit per-endpoint guard on top of that policy, not a replacement for it.

Endpoints are being migrated to authed access incrementally — Campaign, Team, and Stop endpoints are covered; sweep for unauthenticated endpoints before assuming a given route is protected.

### TreeCampaign (core domain) — implemented

Manages campaign seasons, stop assignment, and team dispatch. Receives validated orders from Intake and creates stops.

**Stop ordering** is a read-model concern only: when displaying a team's assigned stops, sort by `StreetSection.SortOrder` (from Territory), then by house number ascending or descending per `StreetSection.Direction`. The old paper route cap of 8 trees is gone — the dispatcher assigns stops to teams dynamically in real time.

**Campaign** carries a `TerritoryRef` (local value object wrapping a Guid, defined in `TreeCampaign.Domain/Campaigns/ExternalReferences/`) to scope address validation to the right territory. This is a correlation ID — Campaign has no compile-time dependency on the Territory domain. It also carries a `ScoutGroupRef` (same pattern) for tenant ownership — see Access above.

**Trailer size**: each `Team` has a trailer capacity, and each `StreetSection` (Territory) declares a `MaxTrailerSize` — the largest trailer that can physically access that section. `TrailerSize` is a shared enum (`Small` | `Large` | `Boogie`, defined in `TreeTerritory.Domain/StreetSections/TrailerSize.cs`) surfaced to TreeCampaign only as data on the read-model projection, not a cross-context type reference. `ClearTrailerFullEndpoint` (Teams) lets the dispatcher clear a team's "trailer full" flag once it's emptied.

### Territory (supporting subdomain) — implemented

The authority on which addresses exist in the service area and how they are traversed. Has no knowledge of payments, stops, or campaigns.

**Domain model:**
```
Territory (aggregate root)
  ├─ ScoutGroupId          ← tenant owner, see Access
  └─ Neighborhood (aggregate root, child of Territory)
        └─ StreetSection (entity, child of Neighborhood)
              ├─ Street reference (by StreetId — Street is a separate aggregate)
              ├─ EvenStartHouseNumber / EvenEndHouseNumber   ← range for even house numbers, independently nullable
              ├─ OddStartHouseNumber / OddEndHouseNumber     ← range for odd house numbers, independently nullable
              ├─ Direction (enum: Ascending | Descending)   ← direction of travel within the section
              ├─ SortOrder (int)        ← position in the neighborhood's route
              └─ MaxTrailerSize (enum: Small | Large | Boogie) ← largest trailer that fits this section

Street (separate aggregate — a street can span multiple neighborhoods)
  ├─ Name
  └─ ZipCode
```

**Why even/odd ranges:** Danish streets commonly have even and odd house numbers on opposite sides with different practical extents (e.g. odd side ends at 27, even side continues to 40). A single `HouseNumberFrom`/`HouseNumberTo` range couldn't express that, so the range is split into an even-number pair and an odd-number pair, each independently optional. Start and end within a pair are also independently nullable — a set start with a null end means "this number and above," a null start with a set end means "this number and below" (the dispatcher often doesn't know the true highest house number on a street). `ContainsHouseNumber` picks the matching pair based on the number's parity and treats a null bound as unbounded on that side.

**Aggregate Boundaries:**
- `Neighborhood` is the aggregate root; `StreetSection` can only be created through `Neighborhood.AddStreetSection()`. The internal `StreetSection.Create()` factory is not publicly accessible.
- `Street` is a separate aggregate referenced by StreetId (no navigation property to Street itself—Territory has no knowledge of Street internals).
- The relation is `Territory → Neighborhood → StreetSection ← Street` (by reference).

**Implementation notes:**
- ID generation: All aggregates auto-generate GUIDs in their factory methods (e.g., `Territory.Create(name)` generates its own TerritoryId).
- `Neighborhood._streetSections` is a backing field with EF Core field access mode; the public `StreetSections` property is read-only.
- Direction enum stored as `byte` for space efficiency.
- Repository pattern: `TreeTerritoryContext` implements `ITreeTerritoryUnitOfWork` and `IRepository<T, TId>` for all aggregates.

**Responsibilities:** validate a raw address against known streets and number ranges; return structured address data and sort metadata to callers; accept "add new street" commands triggered by Intake.

### Intake (supporting subdomain) — domain + infrastructure layers implemented

Turns raw payment messages into validated orders submitted to TreeCampaign.

**Domain model — type-based state machine** (same Zoran Horvat style as Stops):

```
IncomingOrder
  ├─ Accept(ValidationSuccess)       → ValidatedOrder
  ├─ MarkUnwashed()                  → UnwashedOrder      (street not found, or parse failed)
  └─ MarkOutOfBounds(result)         → OutOfBoundsOrder   (street found, house number outside sections)

UnwashedOrder
  ├─ Accept(ValidationSuccess)       → ValidatedOrder     (bulk retry: street now exists in Territory)
  └─ Wash(StreetRef, StreetSectionRef, NeighborhoodRef, HouseNumber) → WashedOrder

WashedOrder
  ├─ Accept(ValidationSuccess)       → ValidatedOrder
  └─ MarkOutOfBounds(result)         → OutOfBoundsOrder

OutOfBoundsOrder
  ├─ Accept(ValidationSuccess)       → ValidatedOrder     (retry after Territory section expanded)
  └─ Transfer(TerritoryRef)          → TransferredOrder   (address belongs to a neighboring scout group's territory)

TransferredOrder
  ├─ UndoTransfer()                  → OutOfBoundsOrder   (transfer was a mistake)
  └─ MarkAsPaid()                    → SettledOrder       (the receiving group has been paid/reimbursed for this order)
```

`OrderBase` carries: `OrderId`, `CampaignRef`, `Sender` (name + phone), `MoneyAmount`, `OrderDate`, `Message` (original free text), `TransactionId?` (from the CSV payment import, see below).

`ValidatedOrder` additionally carries: `HouseNumber`, `StreetRef`, `StreetSectionRef`, `NeighborhoodId` (resolved address components needed to create stops in TreeCampaign).

**Transfer/Settle:** an `OutOfBoundsOrder` whose address falls outside the owning scout group's Territory but inside a neighboring one is `Transfer`red to that `TerritoryRef` rather than retried — it's not a data error, it's someone else's stop to collect. `TransferredOrder` is reversible (`UndoTransfer`) until it is `MarkAsPaid()`'d into a terminal `SettledOrder`, once the inter-group money has actually changed hands. The Intake screen groups outstanding transferred orders by territory with a "settle all" bulk action (`SettleTerritoryOrdersEndpoint`).

**CSV payment import:** `ICsvPaymentParser` / `CsvPaymentParser` (`Intake.Domain/Orders/Services/`) parses a semicolon-delimited MobilePay-style export (columns: `Amount`, `Timestamp`, `Message`, `User Name`, `User Phone Number`, `Payment Transaction ID`) into `PaymentParsingResult` — a discriminated union of `ParsedPayment` | `PaymentParsingFailed(lineNumber, reason)`. It's a pure domain service (no I/O); `PaymentImportService` (`Intake.Application`) is the orchestrator that turns successfully parsed rows into `IncomingOrder`s via the same address-validation pipeline as single orders. Exposed via `POST /api/intake/orders/import`.

**Value objects:**
- `ParsedAddress(Street, HouseNumber, ZipCode?, City?)` — what the auto-parser extracted from `Message`
- Cross-context refs in `ExternalReferences/`: `TerritoryRef`, `NeighborhoodRef`, `StreetRef`, `StreetSectionRef`, `CampaignRef`

**Service interfaces (in `Intake.Domain/Orders/Services/`):**
- `IAddressParser` — `ParsedAddress? TryParse(string message)` — implemented by `RegexAddressParser` in `Intake.Domain` (pure regex, no external dependencies; a domain service)
- `IAddressValidationService` — `Task<AddressValidationResult> ValidateAsync(ParsedAddress, CampaignRef, CancellationToken)` and `Task<AddressValidationResult> ValidateRefsAsync(StreetRef, StreetSectionRef, NeighborhoodRef, HouseNumber, CampaignRef, CancellationToken)` — implemented in `Intake.Application` (application service: coordinates across Territory and TreeCampaign contexts)
- `AddressValidationResult` — sealed discriminated union: `ValidationSuccess(TerritoryRef, NeighborhoodRef, StreetRef, StreetSectionRef, HouseNumber, decimal Latitude, decimal Longitude)` | `StreetNotFound` | `HouseNumberOutOfBounds`
- `IAddressLookupClient` — `Task<AddressResult?> GetAddress(string street, string houseNumber, string zipCode)` — implemented by `DawaClient` in `Intake.Application` (typed `HttpClient` registered via `AddHttpClient<IAddressLookupClient, DawaClient>()`); calls the Danish DAWA address API to resolve coordinates. Injected into `AddressValidationService`.

**Address washing (manual):** When an order is `UnwashedOrder`, the operator corrects the address. The operator submits resolved `StreetRef`, `StreetSectionRef`, `NeighborhoodRef`, and `HouseNumber` via the Intake API — the backend validates these refs against the TreeTerritory domain and calls DAWA (via `IAddressLookupClient`) to fetch coordinates before the `WashedOrder` can be accepted and transition to `ValidatedOrder`.

## Context Interactions

```
Intake ──validates──► Territory       ("is this address in our service area?")
Intake ──submits────► TreeCampaign    ("create a stop for this validated order")
TreeCampaign ──reads► Territory       (projection: sort order, direction, trailer size for stop/team display)
Access ──authenticates► (all)         (login issues the cookie every other context reads via ICurrentUserAccessor)
```

Triggered interactions between the contexts happen through domain events being published via an outbox pattern and then processed by a background worker that distributes events to their respective handlers.

## Project Structure

| Project | Type | Role |
|---|---|---|
| `Common.Domain` | Class Library | Shared domain abstractions: `IDomainEvent`, `IHasDomainEvents` |
| `Common.Infrastructure` | Class Library | Shared infrastructure: `IUnitOfWork`, `IRepository<TAggregate, TId>`, `OutboxDbContext`, `StoredDomainEventContext`, auth (`ICurrentUserAccessor`, `AccessClaimTypes`, `AuthPolicies`) |
| `Access.Domain` | Class Library | Pure domain logic for Access context: `User`, `ScoutGroup` |
| `Access.Infrastructure` | Class Library | EF Core persistence for Access context |
| `Access.Api` | Class Library | Endpoint extension methods for Access context (login/logout/me, user + scout group management) |
| `TreeCampaign.Domain` | Class Library | Pure domain logic — no external dependencies |
| `TreeCampaign.Infrastructure` | Class Library | EF Core + SQL Server, dual DbContext pattern |
| `TreeCampaign.Api` | Class Library | Endpoint extension methods for TreeCampaign context |
| `TreeTerritory.Domain` | Class Library | Pure domain logic for Territory context |
| `TreeTerritory.Infrastructure` | Class Library | EF Core persistence for Territory context |
| `TreeTerritory.Api` | Class Library | Endpoint extension methods for Territory context |
| `Intake.Domain` | Class Library | Pure domain logic for Intake context; includes `RegexAddressParser` (domain service) |
| `Intake.Application` | Class Library | Application services: `AddressValidationService` and future cross-context event handlers |
| `Intake.Infrastructure` | Class Library | EF Core persistence for Intake context |
| `Intake.Api` | Class Library | Endpoint extension methods for Intake context |
| `Host.Api` | ASP.NET Core | Web host — wires up all bounded context endpoints |
| `TreeCampaign.UI` | React/Vite | Frontend SPA |

Dependency direction: `Host.Api` → `*.Api` → `*.Application` → `*.Infrastructure` → `*.Domain` → `Common.Domain`.

Note: `*.Application` projects may reference multiple `*.Infrastructure` projects for cross-context coordination. This is the intended seam where bounded contexts interact.

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

Each transition raises an `IDomainEvent` (from `Common.Domain`). Events are persisted to `StoredDomainEvents` via `OutboxDbContext.SaveChangesAsync` — aggregates and their events are saved in one transaction.

**Event dispatch architecture:**
- Events stored in DB include the event type's `FullName` (e.g., `"TreeCampaign.Domain.Events.StopAssigned"`)
- **`DomainEventHandlerLookup`** (singleton, in `Common.Infrastructure`) uses reflection at startup to scan all `IDomainEventHandler` implementations and build a type registry mapping `FullName` → event type. Keeps handler types cached without instantiating them.
- **`DomainEventDispatcher`** (transient, in `Common.Infrastructure`) receives unprocessed events from DB, resolves their types via the lookup, and uses `IServiceProvider` to instantiate scoped/transient handlers at dispatch time. This ensures handlers respect their configured lifetime and can access DbContexts.
- **`Channel<EventDispatchSignal>`** (singleton, registered in `Host.Api`): injected into the abstract `OutboxDbContext` base class. When events are persisted, a signal is written to the channel to wake the background worker (rather than polling).
- Background worker reads from the channel and calls `DomainEventDispatcher.DispatchDomainEventsAsync()`.

EF Core maps the hierarchy via **Table-Per-Hierarchy (TPH)** with a `StopType` discriminator column. The repository explicitly nulls `AssignedTeamId` when saving an `UnassignedStop` — an invariant EF cannot enforce through TPH alone.

### Data Access

- **`OutboxDbContext`** (in `Common.Infrastructure`) — abstract base for all write contexts; receives `Channel<EventDispatchSignal>` via constructor, handles `StoredDomainEvent` persistence atomically with aggregate saves in one `SaveChangesAsync` transaction, and signals the background worker after persisting events
- **`StoredDomainEventContext`** (in `Common.Infrastructure`) — owns the `StoredDomainEvents` migration; all other contexts call `ExcludeFromMigrations()` for that table
- Each bounded context's write DbContext extends `OutboxDbContext` and implements its own `IUnitOfWork` sub-interface
- Each bounded context has a read-only `ProjectionContext` — `AsNoTracking`, flat DTOs, throws on `SaveChanges`
- All Value Objects have EF Core value converters (e.g., `StopId` ↔ `Guid`) or are configured as complex properties

### IUnitOfWork Convention

Each bounded context defines its own empty sub-interface of `IUnitOfWork` to avoid DI registration conflicts when multiple contexts are registered in one host:

```csharp
public interface ITreeCampaignUnitOfWork : IUnitOfWork { }
public interface ITreeTerritoryUnitOfWork : IUnitOfWork { }
public interface IIntakeUnitOfWork : IUnitOfWork { }
public interface IAccessUnitOfWork : IUnitOfWork { }
```

Each context's `ServiceExtensions` registers only its own sub-interface. Endpoints inject the context-specific interface, never the base `IUnitOfWork`.

### Domain Event Handler Placement

When implementing `IDomainEventHandler<TEvent>`:
- **Handler updates a projection or triggers a technical operation** → `*.Infrastructure`
- **Handler coordinates across bounded contexts** (e.g., `OrderValidated` → create `UnassignedStop`) → `*.Application`

The dispatch infrastructure (background worker, channel notification) is implemented in `Common.Infrastructure` and orchestrated in `Host.Api`.

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

Layered: `client.ts` → hooks → screens → components. React Router v7, routes defined in `src/app/routes.tsx`.

- `/login`, `/register` → unauthenticated
- `/campaigns/:campaignId/teams/:teamId` → Team detail view (tabs: stops, map, info) — unauthenticated so scouts in the field can open it without logging in
- Everything else sits behind `RequireAuth` (`src/app/RequireAuth.tsx`), which gates on the Access login cookie:
  - `/` → Campaign list
  - `/campaigns/:campaignId/dispatch` → Dispatch screen (assign stops to teams)
  - `/campaigns/:campaignId/overview-map` → Overview map screen
  - `/campaigns/:campaignId/intake` → Intake screen (order review, washing, transfer/settle, CSV import)
  - `/territories`, `/territories/:id` → Territory management
  - `/users` → User management screen (Access context)

Vite proxies `/api/*` to `:5006`.

**Dispatch UI principle**: fast decisions under mild pressure. Stops grouped by street/area; dispatcher selects group, selects team. No map required.

**Manual washing UI**: when displaying `UnwashedOrder` records, the frontend calls the TreeTerritory.Api to let the operator select the correct StreetId, StreetSectionId and NeighborhoodId. The operator submits those refs and a house number to the backend via `POST /api/intake/orders/{id}/wash`; the backend then validates the refs against Territory and fetches coordinates from DAWA.

## Technology Stack

- **.NET 10** with C# 13
- **EF Core 10.0.8** with SQL Server (LocalDB in development)
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
