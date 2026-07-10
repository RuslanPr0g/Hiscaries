---
name: project-health
description: Report open GitHub issues/PRs, failing CI runs, pending EF Core migrations per bounded context, Nx-affected projects since the last release tag, stale branches, and outdated/vulnerable packages — one combined health snapshot. Invoke when the user says "project health", "health check", "how healthy is the repo", or "/project-health".
allowed-tools: Bash Read mcp__github__list_issues mcp__github__list_pull_requests mcp__github__get_pull_request_status
---

## Usage

`/project-health`

Produces a single report covering six checks. Run each independently — one check failing (e.g. no network) shouldn't block the others; report what you could gather and note what you couldn't.

## Step 1 — GitHub issues and PRs

Parse `owner`/`repo` from `git remote get-url origin` (strip `git@github.com:`/`https://github.com/` prefix and `.git` suffix). Call `mcp__github__list_issues` (state: open) and `mcp__github__list_pull_requests` (state: open). Summarize counts and flag anything open more than 30 days.

## Step 2 — CI status

```
gh run list --branch master --limit 5 --json conclusion,name,createdAt
```
Report the most recent conclusion per workflow (`.github/workflows/*.yml` — currently `be.build.yml` and any AI PR review workflow). Flag any recent `failure`.

## Step 3 — Pending EF Core migrations, per bounded context

List all six bounded contexts explicitly: PlatformUsers, Stories, Media, Notifications, Recommendations, UserAccounts. Not all of them own a `Persistence.Context` project — check `server/src/` for the current live list (as of this writing, `Media` and `Recommendations` have no EF-backed context and should be reported as "no migrations — no Persistence.Context project", not skipped silently). For the rest, run:
```
dotnet ef migrations has-pending-model-changes --project server/src/Hiscary.<Ctx>.Persistence.Context
```
(Requires the `dotnet-ef` global tool: `dotnet tool install --global dotnet-ef` if the command isn't found.) Report per-context: clean, pending-changes-detected, or not-applicable. This needs the local dev Postgres running (`./scripts/start-backend.sh` or an already-running Aspire session) — if it's not reachable, say so rather than reporting a false "clean."

## Step 4 — Nx-affected projects since the last release tag

```
cd client
LAST_TAG=$(git describe --tags --abbrev=0 2>/dev/null || echo "")
nx show projects --affected --base=${LAST_TAG:-master}
```
If there's no tag yet, fall back to `--base=master` and note that explicitly (affected-since-master, not affected-since-release).

## Step 5 — Stale branches

```
git for-each-ref --sort=-committerdate refs/remotes/origin --format='%(refname:short) %(committerdate:relative)'
```
Flag remote branches with no commits in 60+ days that aren't `master`/`main`.

## Step 6 — Outdated/vulnerable packages

Reuse the same checks as `/dependency-check` (`dotnet list package --outdated --vulnerable` against `Hiscary.sln`, `npm outdated`/`npm audit` against `client/`) but summarize to just counts here — link to `/dependency-check` for the full breakdown rather than duplicating it in full.

## Output format

One report, six sections in the order above, each with a one-line status (✅/⚠️/❌) plus detail. Name all six bounded contexts explicitly in Section 3 even if a context has no pending changes — the point is to show coverage, not just problems.
