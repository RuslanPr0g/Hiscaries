---
name: spec
description: Generate a Spec-Driven Development document (requirements, ADR, solution architecture, Mermaid diagrams, implementation tasks) from a GitHub issue. Invoke when the user says "spec issue #N", "create spec for issue", "design doc for", "SDD for issue", or "write a spec".
allowed-tools: Bash(git *) mcp__github__get_issue mcp__github__get_issue_comments Write Bash(mkdir *)
arguments: [issue]
---

```!
echo "REPO: $(git remote get-url origin 2>/dev/null | sed 's|.*github.com[:/]||;s|\.git$||')"
echo "DATE: $(date +%Y-%m-%d)"
echo "PROJECT_DIR: ${CLAUDE_PROJECT_DIR}"
```

## Inputs

- Issue number: `$issue`
- GitHub repo: taken from `REPO` above (owner/repo format)
- Today's date: taken from `DATE` above
- Project root: taken from `PROJECT_DIR` above

---

## Step 1 — Resolve the output path

Derive the slug:
1. Fetch the issue title (Step 2 below)
2. Lowercase the title, replace spaces and non-alphanumeric characters with hyphens, truncate to 40 characters
3. Prepend the issue number: `{issue_number}-{title-slug}`

Output file: `{PROJECT_DIR}/.claude/specs/{slug}/design.md`

Example: issue #42 titled "Add PDF export for stories" → `42-add-pdf-export-for-stories` → `.claude/specs/42-add-pdf-export-for-stories/design.md`

---

## Step 2 — Fetch issue data

Call `mcp__github__get_issue`:
- Parse `owner` and `repo` from the REPO value above (split on `/`)
- `issue_number` = `$issue`

Call `mcp__github__get_issue_comments` with the same owner/repo/issue_number to capture any clarifications posted in comments.

Extract from the responses: `title`, `body`, `labels`, `assignees`, `comments[].body`.

---

## Step 3 — Analyse before writing

Before generating any text, internally determine the following. Do not write these to the file — use them to drive section content:

- **ISSUE TYPE:** classify as exactly one of: `feature`, `bug`, `refactor`, `performance`, `security`, `spike`
- **BOUNDED CONTEXT:** which of these owns the change: `PlatformUsers`, `Stories`, `Media`, `Notifications`, `Recommendations`, `UserAccounts`, `Shared` (pick the primary one; note secondary if needed)
- **AFFECTED LAYERS:** select all that apply from: `Angular frontend`, `NgRx state`, `.NET API controller`, `Application layer (CQRS)`, `Domain model`, `EF Core/Postgres`, `SignalR/notifications`, `RabbitMQ events`, `Elasticsearch`, `Azure Blob Storage`
- **DIAGRAMS NEEDED:**
  - Sequence diagram: always
  - ER diagram: only if new entity, new column, or new relation
  - Component diagram: only if more than 2 bounded contexts or external services interact
  - Flowchart: only if conditional branching, a state machine, or multi-step async process exists

---

## Step 4 — Write `design.md`

Write the file to the path resolved in Step 1. Use this exact structure. Omit any section marked "omit if not applicable" when it does not apply — do not leave placeholder text. All bracket-enclosed instructions `[like this]` are instructions to you; replace them with real content derived from the issue.

```markdown
# Spec: {issue title} (#{issue_number})

> **Issue:** #{issue_number} · **Repo:** {owner/repo} · **Type:** {ISSUE TYPE} · **Date:** {DATE}
> **Bounded Context:** {context} · **Layers:** {comma-separated affected layers}

---

## 1. Problem Statement

[2–4 sentences extracted from the issue body. State: what pain or gap this addresses, who is affected, and what the current behaviour is vs. the desired behaviour. Do not invent details not present in the issue. If the body is sparse, note that and infer from labels and comments.]

---

## 2. Requirements

### Functional Requirements

[EARS notation: each requirement takes the form "WHEN {trigger}, the system SHALL {observable response}."
Be specific: name the Angular component, API endpoint, domain event, or UI action involved.
Derive from the issue body — do not pad with generic requirements.]

- **FR-01:** WHEN {actor} {trigger}, the system SHALL {observable response}.
- **FR-02:** WHEN {actor} {trigger}, the system SHALL {observable response}.

### Non-Functional Requirements

[Include only NFRs genuinely implied by this issue. Omit if none apply. Candidates: response latency, auth requirement, data integrity, backwards compatibility, rate limiting.]

- **NFR-01:** {system or feature} SHALL {measurable constraint}.

### Out of Scope

[List at least 2 things explicitly excluded to prevent scope creep. Derive from issue context.]

- {excluded concern}
- {excluded concern}

---

## 3. Architecture Decision Record (ADR)

### Status
`Proposed`

### Context

[What technical forces, existing patterns, or constraints shape the implementation choice?
Reference specific project names from this repo where relevant, e.g. `Hiscary.Stories.Application.Write`, `@stories/*` Angular module, `Hiscary.Shared.Jwt`.]

### Decision

[One focused paragraph stating what will be built and the primary technical approach chosen. Be opinionated — avoid hedging phrases like "we could" or "one option is".]

### Alternatives Considered

| Option | Pros | Cons | Rejected because |
|--------|------|------|-----------------|
| {Option A — the approach not taken} | {genuine advantage} | {genuine disadvantage} | {specific reason it loses to the chosen approach} |
| {Option B — another alternative if relevant} | ... | ... | ... |

### Consequences

- **Positive:** {what improves as a result of this decision}
- **Negative:** {what gets harder or more complex}
- **Risks:** {unknowns or dependencies that could invalidate this decision — be honest}

---

## 4. Solution Architecture

### Component Overview

[1–3 paragraphs. Name every .NET project and Angular module that will change. Describe how they interact in plain English. Use exact project names: `Hiscary.{Ctx}.Api.Rest`, `Hiscary.{Ctx}.Application.{Read|Write}`, `Hiscary.{Ctx}.Domain`, `Hiscary.{Ctx}.Persistence.Context`.]

### Sequence Diagram

[Always include. Show the primary happy-path flow from end to end. Include the Angular frontend, LocalApiGateway, the relevant bounded context API, application layer, domain, and persistence. If Notifications are triggered, include the SignalR step. Use exact participant names.]

```mermaid
sequenceDiagram
    actor User
    participant FE as Angular (hiscaries-client)
    participant GW as LocalApiGateway (:5001)
    participant API as Hiscary.{Ctx}.Api.Rest
    participant APP as {Ctx}.Application.Write
    participant DOM as {Ctx}.Domain
    participant DB as PostgreSQL
    Note over FE,DB: {one-line description of what this flow does}
    ...
```

### Component Diagram

[Include only if more than 2 bounded contexts or external services (RabbitMQ, Elasticsearch, Azure Blobs) interact. Omit otherwise.]

```mermaid
graph LR
    ...
```

### Data Flow / State Diagram

[Include only if there is conditional branching, a multi-step async process, or a domain state machine. Omit otherwise.]

```mermaid
flowchart TD
    ...
```

### ER Diagram

[Include only if the data model changes — new entity, new column, new relation. Omit otherwise.]

```mermaid
erDiagram
    ...
```

---

## 5. API Design

[Omit this entire section if no new or changed HTTP endpoints are involved.]

### Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| {METHOD} | /api/v1/{ctx}/{resource} | JWT Bearer | {what it does} |

### Request Schema

```json
{
  "fieldName": "type — what it represents"
}
```

### Response Schema

```json
{
  "fieldName": "type — what it represents"
}
```

---

## 6. Data Model Changes

[Omit this entire section if no EF Core entities or migrations are needed.]

### Changed Entities

[For each entity: name, owning project (`Hiscary.{Ctx}.Domain`), and exact change — new property with type, new navigation property, removed field.]

- `{EntityName}` in `Hiscary.{Ctx}.Domain`: {description of change}

### Migration

```
dotnet ef migrations add {DescriptiveMigrationName} --project server/src/Hiscary.{Ctx}.Persistence.Context
```

---

## 7. Frontend Changes

[Omit this entire section if no Angular code changes.]

### Components / Pages

[For each component: full path under `client/apps/hiscaries-client/src/app/{domain}/`, what it renders, which path alias it imports from.]

- `{domain}/{component}/{component}.component.ts` — {what it does}, imports from `@{alias}/*`

### NgRx Changes

[Only if store changes are needed. List: new actions, new selectors, updated reducer key, new effects and what API calls they make.]

### HTTP Calls

[List new or changed service methods: file path, method name, HTTP verb + endpoint.]

---

## 8. Implementation Tasks

> Tasks are in execution order. Each references the requirement(s) it satisfies.
> Each task is scoped to less than 2 hours of focused work.

- [ ] **TASK-01** `[FR-01]` — {short imperative verb phrase, e.g. "Add CreateX command and handler"}
  - **What:** {name the exact file(s) to create or edit, the class/function to add, and the change to make}
  - **Acceptance:** {specific command to run or action to take that proves it works, e.g. "`dotnet build` passes" or "POST /api/v1/stories/export returns 202"}

- [ ] **TASK-02** `[FR-02]` — {title}
  - **What:** ...
  - **Acceptance:** ...

- [ ] **TASK-NN** `[FR-01, FR-02, NFR-01]` — Write tests
  - **What:** {test file path(s), what scenarios to cover — name the happy path and at least one edge case}
  - **Acceptance:** `nx test hiscaries-client` passes / `dotnet test server/src/Hiscary.{Ctx}.IntegrationTests` passes

---

## 9. Acceptance Criteria

[Restate requirements as Given-When-Then. These are the conditions that close the GitHub issue.]

- [ ] **AC-01:** Given {precondition}, when {actor performs action}, then {observable, testable outcome}.
- [ ] **AC-02:** Given {precondition}, when {actor performs action}, then {observable, testable outcome}.

---

## 10. Open Questions

[List genuine blockers or ambiguities that prevent implementation from starting. If none exist, write the line below and nothing else.]

None — spec is ready for implementation.
```

---

## Step 5 — Confirm output

After writing the file, print exactly:

```
Spec written to .claude/specs/{slug}/design.md
```

Do not print the document content. Do not summarise the sections. Just the confirmation line and the path.
