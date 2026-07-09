# `/ship` skill — design notes

## What it does

Single command to go from local changes → GitHub PR:

```
/ship [optional PR title]
```

Stages → commits (conventional commit) → pushes branch → generates PR body from git log → opens PR via `gh` CLI, with optional GitHub MCP enrichment.

---

## Key design decisions

### Why `disable-model-invocation: true`

This skill has side effects (commits, pushes, creates PRs). Claude must never trigger it automatically. You invoke it explicitly.

### Why `context: fork` is NOT used

Forked subagents lose conversation history. This skill needs to see your current session context (e.g. what feature you just built) to write a meaningful PR description. It runs inline.

### Dynamic context injection (`!` blocks)

```
!`git status --short`
!`git log origin/master..HEAD --oneline`
```

These run **before** Claude sees the skill content. Claude receives the actual git state, not a request to go look it up — faster and more reliable.

### Worktrees

Worktrees are for running **parallel Claude sessions** on separate branches without file collisions:

```bash
claude --worktree feature-auth   # terminal 1
claude --worktree fix-nav-bug    # terminal 2
```

Each worktree is an isolated checkout. When you run `/ship` inside a worktree session, it commits and pushes only that worktree's branch. No coordination needed.

You do NOT need to create worktrees inside the skill — the skill just works on whatever checkout Claude is running in.

### GitHub MCP integration

When the GitHub MCP server is connected, the skill uses it to:
- `mcp__github__list_issues` — find related open issues to auto-link (`Closes #N`)
- `mcp__github__create_pull_request` — alternative to `gh pr create` if CLI unavailable

MCP tools are optional — the skill falls back to `gh` CLI if MCP is not connected.

### Helper script (`scripts/pr-body.sh`)

Claude does not write the PR body template from scratch. The script generates the scaffold (commits, files changed, checklist) from git log. Claude only fills in the Summary section. This keeps the PR body consistent regardless of what Claude decides to write.

Referenced via `${CLAUDE_PROJECT_DIR}` substitution so the path resolves correctly whether you start Claude from the repo root or a subdirectory.

---

## Skill structure

```
.claude/skills/ship/
├── SKILL.md          ← main instructions, dynamic git context injected at load time
├── DESIGN.md         ← this file (not loaded by Claude, reference only)
└── scripts/
    └── pr-body.sh    ← generates PR body template from git log
```

---

## Setup requirements

| Requirement | Status |
|---|---|
| `gh` CLI authenticated (`gh auth login`) | must do once |
| GitHub MCP server configured | optional (TASK-13 in setup plan) |
| `GITHUB_TOKEN` env var | needed for MCP only |
| commitlint config at `client/commitlint.config.cjs` | already present |

---

## Extending this skill

**To add Jira ticket linking:** add a `!` block that reads from a `.jira-ticket` file or an env var and appends `Jira: PROJ-123` to the PR body.

**To support draft PRs:** add a `--draft` flag check on `$ARGUMENTS` and pass `--draft` to `gh pr create`.

**To auto-assign reviewers:** append `--reviewer <github-username>` to the `gh pr create` command.

---

## Docs refs

- [Skills frontmatter reference](https://code.claude.com/docs/en/slash-commands#frontmatter-reference)
- [Dynamic context injection](https://code.claude.com/docs/en/slash-commands#inject-dynamic-context)
- [Worktrees](https://code.claude.com/docs/en/worktrees)
- [GitHub MCP](https://docs.anthropic.com/en/docs/claude-code/mcp)
- [disable-model-invocation](https://code.claude.com/docs/en/slash-commands#control-who-invokes-a-skill)
