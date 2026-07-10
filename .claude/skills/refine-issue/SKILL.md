---
name: refine-issue
description: Take an existing GitHub issue and interactively ask clarifying questions to tighten its requirements, technical design, edge cases, and acceptance criteria — then update the issue so it's ready for /spec. Invoke when the user says "refine issue #N", "improve this issue", "clarify issue", "tighten up issue #N", or "/refine-issue".
allowed-tools: Bash(git *) mcp__github__get_issue mcp__github__get_issue_comments mcp__github__update_issue AskUserQuestion
arguments: [issue]
---

```!
git remote get-url origin
```

## Inputs

- Issue number: `$issue`
- GitHub repo: parse `owner/repo` from the URL printed above (strip the `git@github.com:`/`https://github.com/` prefix and the trailing `.git`)
- Today's date: use the current date already available in session context

If `$issue` is missing, ask the user for the issue number (or a URL, from which extract the number) before doing anything else.

---

## Step 1 — Fetch the issue

Call `mcp__github__get_issue` (owner/repo parsed from `REPO`, `issue_number` = `$issue`), then `mcp__github__get_issue_comments` for the same issue to pick up any clarification already given in comments — don't re-ask what's already answered there.

Extract `title`, `body`, `labels`, `assignees`.

---

## Step 2 — Classify and find the gaps

Determine internally (do not print this analysis verbatim, use it to drive Step 3):

- **ISSUE TYPE:** `feature`, `bug`, `refactor`, `performance`, `security`, `chore`, `spike` — infer from title, body, and labels.
- **CURRENT COMPLETENESS:** for each of the checklist items below, mark present / vague / missing by actually reading the body — don't assume missing just because a heading is absent, the info may be inline.

Checklist by type:

**All types**
- Clear problem statement / motivation (the "why")
- Explicit scope boundary (what's excluded)
- Acceptance criteria that are objectively testable

**feature / performance / spike**
- Target actor(s) / persona and trigger
- Desired observable behavior (not just a vague ask)
- UI/UX expectations, if user-facing
- Technical constraints or dependencies (existing systems, bounded contexts, external services it touches)
- Non-functional expectations (perf, auth, data integrity) if plausibly relevant

**bug**
- Steps to reproduce
- Expected vs. actual behavior
- Environment (browser, device, backend version, data state) if relevant
- Regression info — did this used to work, since when
- Severity / frequency (always, intermittent, specific condition)
- Relevant logs, stack traces, or screenshots referenced

**refactor / chore**
- What triggers the need (tech debt, blocking another change, etc.)
- Boundaries of the change (files/modules affected, what must NOT change behaviorally)
- How correctness will be verified post-refactor (tests, no behavior change confirmation)

Only items marked vague/missing become candidate questions. If everything is already well specified, skip to Step 5 and tell the user the issue looks solid, asking only to confirm before making any small polish edits.

---

## Step 3 — Ask clarifying questions

Turn each vague/missing checklist item into a concrete question using `AskUserQuestion`. Rules:

- Batch up to 4 questions per `AskUserQuestion` call. If more than 4 gaps exist, run multiple rounds sequentially (don't dump everything in the user's face at once — do the most decision-critical round first: problem/requirements before technical design before edge cases).
- Every question must be **specific to this issue's actual content** — never generic boilerplate like "what is the priority?" unless priority genuinely changes the design.
- Give 2-4 concrete, mutually exclusive options grounded in the issue and this repo's architecture (bounded contexts, Angular modules, etc. from `CLAUDE.md`) where a reasonable guess is possible — the user can always pick "Other" to give a free-text answer. Don't force an option list where the honest answer is open-ended; still phrase the question so a short free-text reply is natural.
- For bug reports missing repro steps, ask directly for the steps rather than offering guessed options.
- Skip asking about anything already answered in the issue body or its comments.

Keep going through rounds until the checklist items that matter for writing requirements and a technical design are resolved. Stop early if the user signals they want to wrap up (e.g. "that's enough", "just use your best judgment") — fill remaining gaps with clearly-marked assumptions instead of blocking further.

---

## Step 4 — Synthesize the refined issue

Rewrite the issue body (keep the title unless it's misleading — if so, propose a new one and confirm with the user in the same message rather than a separate question round) using this structure. Merge the original content with the answers gathered — don't just append a Q&A transcript.

```markdown
## Problem / Motivation

[Clear statement of the why, the pain point, who's affected — from original body + answers]

## Current vs. Desired Behavior
[Omit for pure feature requests with no "current" state. Required for bugs: expected vs actual.]

## Steps to Reproduce
[Bugs only. Numbered steps, omit entirely for non-bugs.]

## Requirements

- {specific, testable requirement}
- {specific, testable requirement}

## Technical Considerations

[Constraints, affected bounded context(s)/modules, dependencies, integration points — whatever surfaced from the technical-design questions. Omit if genuinely nothing beyond the obvious.]

## Out of Scope

- {explicitly excluded concern}
- {explicitly excluded concern}

## Acceptance Criteria

- [ ] {specific, testable outcome}
- [ ] {specific, testable outcome}

## Open Assumptions
[Only include if Step 3 was cut short and you filled gaps with assumptions instead of answers. List each assumption plainly so it can be corrected later. Omit this section otherwise.]
```

Show the user the refined body as a preview (in the chat, not yet written to GitHub) and ask them to confirm, tweak, or approve before publishing — updating a GitHub issue is a visible, shared-state change, so don't post it without explicit go-ahead.

---

## Step 5 — Update the issue

Once the user confirms, call `mcp__github__update_issue`:
- `owner`/`repo` from `REPO`
- `issue_number` = `$issue`
- `title`: new title if changed, otherwise omit
- `body`: the confirmed refined body

Preserve existing `labels`/`assignees` — do not touch them unless the user explicitly asked to change them.

---

## Step 6 — Report

Print a short confirmation:

```
Issue #{issue} refined: {issue_url}
Ready for: /spec issue #{issue}
```

Do not restate the full body — the user already saw and approved it in Step 4.
