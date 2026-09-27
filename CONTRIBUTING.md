# Contributing

Thanks for your interest in contributing!

## Workflow

1. Fork or branch from `master`.
2. Make your changes on a feature branch (e.g. `feature/your-change`).
3. Open a pull request into `master`.
4. At least **1 approving review** is required before merge.
5. Direct pushes and force pushes to `master` are disabled — all changes must go through a PR.

## CI Checks

On push/PR to `master`: .NET CI with unit + integration tests (`be.build.yml`), Angular CI with lint, format check, unit tests, and build (`fe.build.yml`). Every PR also gets an automated AI review (`pr-review.yml`) — unresolved critical/high findings from a previous run will fail the PR, so address review comments before re-requesting review. `release.yml` handles releases on push.
