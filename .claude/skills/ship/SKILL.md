---
name: ship
description: Commit staged/unstaged changes, push to remote, and open a pull request with an auto-generated description. Use when the user says "ship", "create a PR", "commit and push", or "open a pull request".
disable-model-invocation: true
allowed-tools: Bash(git *) Bash(gh *) Bash(${CLAUDE_PROJECT_DIR}/.claude/skills/ship/scripts/*.sh)
arguments: [title]
---

## Current repo state

```!
git status --short
git log origin/master..HEAD --oneline 2>/dev/null || git log HEAD~3..HEAD --oneline
```

## Instructions

You are shipping the current changes to GitHub. Follow these steps exactly:

### 1. Validate state

Run `git status --short`. If working tree is clean and no commits ahead of remote, tell the user there is nothing to ship and stop.

### 2. Determine branch

- If already on a `feature/`, `fix/`, or `chore/` branch that is not `master`: use it.
- If on `master` or a branch with no prefix: ask the user for a branch name, then run:
  `git checkout -b <branch-name>`

### 3. Stage and commit

- Run `git status` to show what will be staged.
- Stage all tracked changes: `git add -u`
- For untracked files: show them and ask which to include.
- Write a conventional commit message based on the diff:
  - `feat:` for new functionality
  - `fix:` for bug fixes
  - `chore:` for tooling/config
  - `refactor:` for restructuring without behaviour change
- Commit: `git commit -m "<message>"`
- If commitlint hook fails, fix the message format and retry.

### 4. Push

```
git push -u origin HEAD
```

### 5. Generate PR description

Run the helper script to build the PR body from git log:

```
${CLAUDE_PROJECT_DIR}/.claude/skills/ship/scripts/pr-body.sh
```

Use its output as the PR body template. Enhance the "## Summary" section with context from the diff — what changed and why. Keep it concise.

If `$title` was provided as an argument, use it as the PR title. Otherwise derive the title from the most recent commit subject (max 70 chars).

### 6. Create the PR

Ensure the "ai" label exists before creating the PR:
```
gh label create "ai" --color "0075ca" --description "AI-assisted change" 2>/dev/null || true
```

Create the PR with assignee and label:
```
gh pr create \
  --title "<title>" \
  --body "<body from step 5>" \
  --base master \
  --assignee @me \
  --label "ai"
```

Create the PR using this priority order:

1. **MCP first** — use `mcp__github__create_pull_request` (owner/repo parsed from `git remote get-url origin`). Then add assignee and label via `gh pr edit <url> --add-assignee @me --add-label "ai"` if `gh` is available.
2. **gh fallback** — only if MCP fails or is unavailable: use `gh pr create --title ... --body ... --base master --assignee @me --label "ai"`.

Also use `mcp__github__list_issues` to check for a matching open issue and append `Closes #<N>` to the PR body if found.

### 7. Report

Print:
- PR URL
- Branch name
- Commit SHA
- Files changed (count)
