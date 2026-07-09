---
name: automation-ideas
description: Brainstorm at least 10 automation ideas for this project across three tiers (plain non-AI automation, AI-assisted SDLC support, AI agentic loops), then create one GitHub issue per idea the user picks. Invoke when the user says "automation ideas", "how could we automate this project", "suggest some automations", or "/automation-ideas".
allowed-tools: mcp__github__create_issue mcp__github__list_issues
---

```!
echo "REPO: $(git remote get-url origin 2>/dev/null | sed 's|.*github.com[:/]||;s|\.git$||')"
```

## Step 1 — Generate ideas

Produce **at least 10 ideas total**, spread across three tiers. Ground every idea in something real about this repo — its Aspire backend, Angular/Nx frontend, RabbitMQ/SignalR, CI workflows, bounded contexts — not generic boilerplate. Skim `.github/workflows/`, `CLAUDE.md`, and recent PR/issue activity first if useful context is missing.

### Tier 1 — Plain automation (no AI, deterministic, cheap to build)

Few-shot examples (calibrate scope and tone — this specific, not vaguer):

- **PR-open Slack/Discord ping** — GitHub Action posts to a webhook when a PR opens or goes ready-for-review, so nobody has to poll GitHub.
- **Stale-branch reminder** — weekly workflow comments on PRs with no activity for 14+ days and pings the author.
- **Path-based auto-labeler** — label PRs `frontend`/`backend`/`infra` automatically based on which top-level directory changed.
- **Migration-file guard** — CI check that fails a PR if it hand-edits a file under `*.Persistence.Context/Migrations/`.
- **Dependency update bot** — Renovate/Dependabot configured for `client/package.json` and the `.csproj` files, grouped by ecosystem.

### Tier 2 — AI-assisted SDLC support (human-triggered, AI does the work)

Few-shot examples:

- **AI PR description filler** — on PR open with an empty body, a bot generates a summary from the diff and posts it as a suggested description.
- **AI flaky-test triage** — when CI fails, a workflow asks a model to read the failure log and post a comment guessing root cause + which file to look at.
- **AI changelog drafter** — on release tag push, generate `CHANGELOG.md` entry from merged PR titles since the last tag.
- **AI code-review second pass** — after human approval, run `/code-review` in CI and post findings as a non-blocking comment.

### Tier 3 — AI agentic loops (triggered or scheduled, multi-step, low human involvement)

Few-shot examples:

- **Nightly dead-code sweep** — scheduled agent scans for unused Angular components/exports and unreferenced .NET classes, files one issue per cluster found (not one per file).
- **CI-failure auto-fix loop** — on a failed build on a non-protected branch, an agent reproduces the failure, attempts a minimal fix, and opens a draft PR against the original branch for human review.
- **Issue-to-spec agent** — on issue label `needs-spec`, automatically invoke the `spec` skill and post a link to the generated doc as a comment.
- **Dependency-bump verifier loop** — when Dependabot opens a PR, an agent checks it out, runs the affected test suites, and either approves or comments with the failure.

### Output format for Step 1

Present ideas compactly — **do not** write the detailed version yet:

```
### Tier 1 — Plain automation
1. **Title** — one-sentence hook.
2. ...

### Tier 2 — AI-assisted SDLC support
...

### Tier 3 — AI agentic loops
...
```

Number ideas sequentially across all tiers (1–10+) so they're easy to reference.

---

## Step 2 — Get the user's picks

Use `AskUserQuestion` (multiSelect) or a plain follow-up asking which numbered ideas to turn into issues. Do not create any issue before the user has picked.

---

## Step 3 — Create one GitHub issue per selected idea

For each idea the user picked, expand the one-liner into a detailed issue body — the compact pitch from Step 1 is not enough for an issue. Use this structure:

```markdown
## Problem / Motivation

[Why this is worth automating — what manual toil or risk it removes, grounded in this repo's actual workflow.]

## Proposed Approach

[Concrete mechanism: which GitHub Action/trigger, which script or model call, what it reads and where it writes/posts. Name real paths — e.g. `.github/workflows/`, `server/src/Hiscary.<Ctx>`, `client/apps/hiscaries-client` — where applicable.]

## Tier

[Tier 1 — Plain automation | Tier 2 — AI-assisted SDLC support | Tier 3 — AI agentic loop]

## Acceptance Criteria

- [ ] {specific, testable outcome}
- [ ] {specific, testable outcome}
```

Call `mcp__github__create_issue` once per idea:
- `owner`/`repo`: parsed from `REPO` above
- `title`: the idea's title (no numbering prefix)
- `body`: the expanded body above
- `labels`: `["automation-idea"]` plus one of `["tier-1-automation", "tier-2-ai-assist", "tier-3-agentic-loop"]` matching the tier

If a label doesn't exist yet, `create_issue` will still apply it as text — GitHub auto-creates unknown labels on issue creation via the API in most cases; if it errors, retry without that label and note it in the report.

---

## Step 4 — Report

List, per created issue: title, tier, and issue URL. Do not restate the full body.
