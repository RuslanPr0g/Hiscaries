---
name: find-untested
description: Cross-reference Application write-service methods / Angular components against existing test files, ranked by how central each file is (referenced-by count), to surface the highest-leverage places to add tests next. Invoke when the user says "find untested code", "what's untested", "test coverage gaps", or "/find-untested".
allowed-tools: Bash Read Grep Glob
---

## Usage

`/find-untested [backend|frontend]`

No argument runs both. This does not compute line/branch coverage — it's a structural cross-reference: which write-service methods / components have zero corresponding test file, ranked by an approximate "how many other files reference this one" centrality score, so the highest-blast-radius gaps surface first.

## Backend pass

1. Enumerate write-service classes: `find server/src -path '*.Application.Write/Services/*.cs' -not -path '*/obj/*' -not -path '*/bin/*'`. For each, list its public methods (the "use cases" per `.claude/rules/backend/cqrs-ddd.md`).
2. Enumerate existing unit test files: `find server/src -path '*.Domain.Tests/*.cs' -not -path '*/obj/*' -not -path '*/bin/*'` (and `*.Application.Tests/*.cs` once any such project exists — none does yet). Also note integration test coverage from `*.IntegrationTests/Scenarios/*.cs` — a method exercised only by an integration test is "covered end-to-end" but not "unit tested"; report both signals separately, don't conflate them.
3. For each write-service method, grep the test files for the containing class name or method name to approximate whether it has a corresponding test. This is a heuristic, not exact — report it as such.
4. Rank by centrality: count how many other projects/files reference the aggregate root or service (`grep -rl` across `server/src` for the type name, excluding the defining project). More references = higher blast radius if untested.

## Frontend pass

1. Enumerate components: `find client/apps/hiscaries-client/src/app -name '*.component.ts' -not -name '*.spec.ts'`.
2. For each, check whether a co-located `*.component.spec.ts` and/or `*.property.spec.ts` exists (per `.claude/rules/frontend/unit-testing.md`).
3. Rank by centrality: how many other `.ts` files import the component (grep for its class name / selector across `client/apps/hiscaries-client/src/app`, excluding its own file).

## Output format

Two ranked tables (backend, frontend), each sorted by centrality descending, columns: `File | Tested? | Referenced by (count)`. Cap the report at the top 15 untested-and-most-referenced entries per pass so it stays actionable — don't dump the entire codebase.

## Verifying against this repo

As of the xUnit v3 migration (`.claude/specs/claude-code-config-plan-rules-skills/design.md` TASK-06), `Hiscary.UserAccounts.Domain.Tests` covers `UserAccount.Ban`/`ValidateRefreshToken` — that project's classes should report as "tested" in the backend pass. Every other bounded context's `Application.Write` services currently have zero unit test files and should surface as untested, ranked by centrality, in the same pass — this is the expected baseline until the pattern from TASK-06 is repeated per §Out of Scope of that spec.
