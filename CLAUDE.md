# Hiscaries — Claude Context

## Project Overview

Hiscaries is a full-stack storytelling platform. The frontend is an Angular 18+ SPA using NgRx for state management and SignalR for real-time notifications. The backend is a .NET 9 solution orchestrated by .NET Aspire, structured around six bounded contexts with DDD + CQRS, backed by PostgreSQL and EF Core.

## Directory Layout

| Path | Contents |
|------|----------|
| `client/` | Nx Angular monorepo — one app (`hiscaries-client`) and shared libs |
| `server/src/` | .NET Aspire solution — 6 bounded contexts, each with API / Application / Domain / Persistence layers |
| `scripts/` | Dev automation shell scripts |
| `.claude/` | Claude Code configuration (settings, hooks, MCP) |
| `.github/` | CI/CD workflows |

## Running the Project

| Command | What it does |
|---------|-------------|
| `./scripts/start.sh` | Start full stack (backend + frontend) |
| `./scripts/start-backend.sh` | Start .NET Aspire backend only |
| `./scripts/start-frontend.sh` | Start Angular dev server only |
| `./scripts/host.sh` | Start in LAN/host mode (binds to 0.0.0.0) |
| `./scripts/reset-to-master.sh` | Reset to master, pull, prune remote branches |

## Conventions

- **Commits:** conventional commits enforced by commitlint (`client/commitlint.config.cjs`)
- **Branches:** `feature/`, `fix/`, `chore/` prefixes
- **EF Core migrations:** generated files in `*.Persistence.Context` projects — do not hand-edit

## Off-Limits Paths

Never manually edit or commit files under these paths:

- `server/src/*/obj/`
- `server/src/*/bin/`
- `client/node_modules/`
- `client/dist/`
- `server/src/azurite-data/`
- `server/src/elasticsearch-data/`
- `client/migrations.json`
