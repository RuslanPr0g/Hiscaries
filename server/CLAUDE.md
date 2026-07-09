# Server — Claude Context

## Startup

- Framework: **.NET 9** with **.NET Aspire** orchestration
- Single entry point: `Hiscary.AppHost` — it starts all services
- Start via script: `./scripts/start-backend.sh`
- Start directly: `dotnet run --project server/src/Hiscary.AppHost`
- Local infrastructure spun up automatically: PostgreSQL, Azurite (Azure Storage), Elasticsearch

Local data directories (do not edit):
- `server/src/azurite-data/` — Azure Storage emulator
- `server/src/elasticsearch-data/` — Elasticsearch

API routing in local dev goes through `Hiscary.LocalApiGateway`.

## Architecture

Pattern per bounded context:

```
{Ctx}.Api.Rest
  → {Ctx}.Application.Read / .Write
    → {Ctx}.Domain
      → {Ctx}.Persistence.Read / .Write / .Context
```

**Rule:** `.Api.Rest` projects are thin controllers only — no business logic there.

### Bounded Contexts

| Context | Responsibility |
|---------|---------------|
| PlatformUsers | Platform-level user profiles |
| Stories | Core story content |
| Media | File/image/document handling |
| Notifications | Real-time and async notifications |
| Recommendations | Story/user recommendations |
| UserAccounts | Auth, registration, account management |

### Shared Infrastructure (`Hiscary.Shared.*`)

- `Hiscary.Shared.Domain` — base domain primitives
- `Hiscary.Shared.Application` — shared application utilities
- `Hiscary.Shared.Persistence.EF.Postgres` — EF Core + PostgreSQL base
- `Hiscary.Shared.Api.Rest` — shared REST utilities
- `Hiscary.Shared.Jwt` — JWT issuance and validation
- `Hiscary.ServiceDefaults` — Aspire service defaults (telemetry, health checks)

### Event-Driven Layers (per context)

- `{Ctx}.DomainEvents` — domain event definitions
- `{Ctx}.IntegrationEvents` — integration event definitions
- `{Ctx}.EventHandlers` — event handler processors
- `{Ctx}.Jobs` — background job workers

### Supporting Projects

- `Hiscary.Media.Images` — image processing
- `Hiscary.Media.FileStorage` — file storage abstraction
- `Hiscary.Media.DocumentTools.*` — PDF/document generation (PdfPig, QuestPDF, PuppeteerSharp)

## Database

- ORM: **EF Core** with **PostgreSQL**
- Local Postgres runs in Docker via Aspire, always on **port 5432**
- Password is static, stored in .NET User Secrets (never committed): `dotnet user-secrets set "Parameters:postgres-password" "<pass>" --project server/src/Hiscary.AppHost`
- Local connection string: `postgresql://postgres:Hisc4ryDev!2025@localhost:5432/postgres`
- Migrations live in `{Ctx}.Persistence.Context` projects
- Add a migration:
  ```
  dotnet ef migrations add <MigrationName> --project server/src/Hiscary.<Ctx>.Persistence.Context
  ```
- Do not hand-edit generated migration files

## Auth

- JWT issued and validated in `Hiscary.Shared.Jwt`
- Angular client decodes tokens via `@auth0/angular-jwt`

## Real-Time

- SignalR hub: `Hiscary.Notifications.SignalR`
- Angular client connects via `@microsoft/signalr`

## Integration Tests

- Base helpers: `Hiscary.IntegrationTesting`, `Hiscary.Shared.IntegrationTesting`
- Example test project: `Hiscary.UserAccounts.IntegrationTests`
