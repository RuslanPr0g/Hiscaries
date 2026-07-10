---
name: new-bounded-context
description: Scaffold a brand-new bounded context (all backend layer projects, solution wiring, API gateway route, and the matching Angular feature folder). Invoke when the user says "new bounded context", "scaffold context", "add a bounded context", or "/new-bounded-context <ContextName>".
allowed-tools: Bash Read Write Edit Glob Grep
---

## Usage

`/new-bounded-context <ContextName>`

`<ContextName>` is PascalCase and must not already exist under `server/src/Hiscary.<ContextName>.*` or `client/apps/hiscaries-client/src/app/<kebab-case-name>/`.

This scaffolds every layer project for the context, wires it into `Hiscary.sln` and `Hiscary.LocalApiGateway`, and scaffolds the matching Angular feature folder + path alias — mirroring the six existing bounded contexts (PlatformUsers, Stories, Media, Notifications, Recommendations, UserAccounts). Read `server/CLAUDE.md` and `.claude/rules/backend/{ef-core,cqrs-ddd}.md` before generating any code so the scaffold matches current conventions, not stale assumptions.

## Step 1 — Confirm the name and layer set

Ask the user (if not already clear) which layers the new context needs. A full context has:

```
Hiscary.<Ctx>.Api.Rest
Hiscary.<Ctx>.Application.Read
Hiscary.<Ctx>.Application.Write
Hiscary.<Ctx>.Domain
Hiscary.<Ctx>.DomainEvents
Hiscary.<Ctx>.IntegrationEvents
Hiscary.<Ctx>.EventHandlers
Hiscary.<Ctx>.Jobs
Hiscary.<Ctx>.Persistence.Context
Hiscary.<Ctx>.Persistence.Read
Hiscary.<Ctx>.Persistence.Write
Hiscary.<Ctx>.Domain.Tests
```

Not every context needs every layer (e.g. `Recommendations` has no `.Api.Rest`... actually check the live repo before assuming — some contexts diverge). Default to the full set above unless the user says otherwise.

## Step 2 — Scaffold backend projects

Use an existing context as the structural template — `Hiscary.Notifications.*` is a good reference since it has all layers including SignalR/Jobs. For each layer:

1. `mkdir server/src/Hiscary.<Ctx>.<Layer>`
2. Copy the `.csproj` shape from the matching `Hiscary.Notifications.<Layer>.csproj` (same `PackageReference`/`ProjectReference` items, adjusted for the new context name), not a blind text copy — check whether the reference project actually needs each package for a fresh domain.
3. `Domain` gets no dependencies beyond `Hiscary.Shared.Domain` and `StackNucleus.DDD.Domain`.
4. `Application.Read`/`Application.Write` reference `Domain` only (not `Persistence.*` implementations).
5. `Persistence.Context` derives its `DbContext` from `BaseNucleusContext<TContext>` per `.claude/rules/backend/ef-core.md`, sets a unique `SCHEMA_NAME`, and gets a `NucleusDatabaseDesignTimeDbContextFactory<TContext>` subclass.
6. `Api.Rest` uses `Microsoft.NET.Sdk.Web`, references `Hiscary.Shared.Api.Rest`, `Hiscary.ServiceDefaults`, and all the context's own layer projects — controllers only, no business logic, per root `CLAUDE.md`.
7. `Domain.Tests` follows `.claude/rules/backend/unit-testing.md`: `xunit.v3`, `NSubstitute`, `Microsoft.Testing.Extensions.CodeCoverage`, `IsTestProject=true`.

Add every new project to the solution:
```
dotnet sln server/src/Hiscary.sln add server/src/Hiscary.<Ctx>.<Layer>/Hiscary.<Ctx>.<Layer>.csproj
```

## Step 3 — Wire into Hiscary.AppHost and Hiscary.LocalApiGateway

- In `Hiscary.AppHost/Program.cs`, add a `builder.AddProject<Projects.Hiscary_<Ctx>_Api_Rest>("hc-<ctx-lowercase>-api-rest")` entry alongside the existing six, wired to the same Postgres/RabbitMQ resources they depend on.
- In `Hiscary.LocalApiGateway/appsettings.json`, add a new route + cluster entry under `ReverseProxy.Routes` / `ReverseProxy.Clusters`, following the existing `/api/v1/<domain>/{**catch-all}` path convention and pointing the cluster destination at the new service's local port.

## Step 4 — Scaffold the Angular feature folder

1. `mkdir -p client/apps/hiscaries-client/src/app/<kebab-case-name>/{services,components,store}` — omit `store/` unless this context needs shared/async state beyond what a signals-based service covers (see `.claude/rules/frontend/ngrx.md` — NgRx is not the default).
2. Add the path alias to `client/tsconfig.base.json` under `paths`: `"@<name>/*": ["apps/hiscaries-client/src/app/<kebab-case-name>/*"]`, matching the existing `@stories/*`, `@shared/*`, etc. entries.
3. Scaffold at least one standalone service (`OnPush`, `inject()`-based DI) per `.claude/rules/frontend/angular-conventions.md` if the user specified any initial API surface.

## Step 5 — Verify

```
dotnet build server/src/Hiscary.sln
cd client && nx lint hiscaries-client
```

Report the list of created files/projects and both command results. If either fails, stop and report the exact error — don't silently continue with a half-wired context.

## Cleanup for throwaway/dry-run invocations

If the user is only testing the skill (a throwaway context name), offer to remove the generated projects/folders and revert the `.sln`/`tsconfig.base.json`/gateway config edits afterward rather than leaving scaffold debris in the repo.
