---
name: reset-dev-db
description: Drop and recreate the local dev Postgres database, then reapply every bounded context's EF Core migrations in dependency order. Refuses to run against anything but localhost. Invoke when the user says "reset dev db", "reset local database", "wipe my local postgres", or "/reset-dev-db".
allowed-tools: Bash Read
---

## Usage

`/reset-dev-db [--seed]`

## Hard safety rule — read this first

**This skill must refuse to run against anything but the local dev connection string.** Before doing anything destructive, resolve the connection string the same way `Hiscary.AppHost` does (via `dotnet user-secrets list --project server/src/Hiscary.AppHost`, key `Parameters:postgres-password`, host always `localhost:5432` per `server/CLAUDE.md`) and verify the host is literally `localhost` or `127.0.0.1`. If it resolves to anything else — a remote host, an `.env`-provided override pointing elsewhere, or the secret is missing entirely — **stop and report the resolved host without executing any drop/recreate command.** Never accept a connection string from a flag or environment variable without this check; a typo'd `POSTGRES_HOST` pointing at a shared or prod-like environment must not be able to bypass the guard.

## Step 1 — Confirm the target

```
dotnet user-secrets list --project server/src/Hiscary.AppHost | grep postgres-password
```
Print the resolved host (should be `localhost:5432`) and ask the user to confirm before proceeding if this is the first time the skill has been run in a session (subsequent invocations in the same session don't need to re-confirm).

## Step 2 — Drop and recreate the database

The local Postgres container is Aspire-managed, so the cleanest reset is via `psql` against the running container rather than restarting the container itself (restarting would also reset volumes for other Aspire-managed services unnecessarily):

```
docker exec -it $(docker ps --filter "name=hiscary-" --format "{{.Names}}" | head -1) \
  psql -U postgres -c "DROP DATABASE IF EXISTS postgres;" -c "CREATE DATABASE postgres;"
```

If no `hiscary-*` container is running, tell the user to start the backend first (`./scripts/start-backend.sh`) — this skill resets an already-running local Postgres, it doesn't start Aspire itself.

## Step 3 — Reapply migrations for all six bounded contexts, in dependency order

Run `dotnet ef database update` per context. Order matters where contexts reference shared schema setup — run `UserAccounts` and `PlatformUsers` first (identity/user data other contexts' seed data may assume exists), then the rest:

```
dotnet ef database update --project server/src/Hiscary.UserAccounts.Persistence.Context
dotnet ef database update --project server/src/Hiscary.PlatformUsers.Persistence.Context
dotnet ef database update --project server/src/Hiscary.Stories.Persistence.Context
dotnet ef database update --project server/src/Hiscary.Notifications.Persistence.Context
```

`Media` and `Recommendations` currently have no `Persistence.Context` project (no EF-backed schema as of this writing) — skip them but say so explicitly rather than silently omitting two of the six contexts from the report.

## Step 4 — Optional seeding

If `--seed` was passed, run any existing `*Seeder` classes (e.g. `Hiscary.Stories.Persistence.Context/Seeds/GenreSeeder.cs`) that are already wired into `OnModelCreating`/startup — EF's `HasData` seeds apply automatically on `database update`, so this step is usually a no-op unless a context has a separate manual seeding entry point. Check before assuming one exists.

## Step 5 — Report

Confirm all four EF-backed contexts are at their latest migration (`dotnet ef migrations list --project ... | tail -1` should match the newest migration file) and note the two schema-less contexts. If any `database update` fails, stop and report the exact error — don't proceed to the next context with a half-applied schema.
