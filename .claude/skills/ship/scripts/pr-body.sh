#!/usr/bin/env bash
# Generates a PR body template from git log since master.
# Claude fills in the Summary section; the rest is scaffolded.

BASE=${1:-master}
COMMITS=$(git log origin/$BASE..HEAD --oneline 2>/dev/null || git log HEAD~5..HEAD --oneline)
FILES=$(git diff --name-only origin/$BASE..HEAD 2>/dev/null || git diff --name-only HEAD~5..HEAD)
AUTHOR=$(git config user.name)

cat <<EOF
## Summary

<!-- Claude: fill this in based on the diff -->

## Commits

\`\`\`
$COMMITS
\`\`\`

## Changed files

\`\`\`
$FILES
\`\`\`

## Test plan

- [ ] Ran \`nx affected:test --base=$BASE\` (frontend)
- [ ] Verified build passes: \`nx build hiscaries-client\`
- [ ] Manually tested the golden path

---
🤖 Generated with [Claude Code](https://claude.ai/claude-code) · Author: $AUTHOR
EOF
