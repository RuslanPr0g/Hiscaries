# Hiscaries Frontend (Angular + Nx)

## Prerequisites

- Node.js 20.x
- npm 10.x

You can use `.nvmrc` in this folder:

```bash
nvm use
```

## Quick Start

```bash
npm ci
npm run start
```

The app runs with `proxy.conf.json`, so backend services should be available for full functionality.

## Common Commands

- `npm run start` - start frontend dev server.
- `npm run start:backup` - start frontend on backup port `4002`.
- `npm run lint` - run app lint checks (no auto-fix).
- `npm run lint:fix` - run lint and apply safe fixes.
- `npm run lint:all` - lint the full workspace.
- `npm run format` - apply Prettier formatting.
- `npm run format:check` - fail if any file violates Prettier rules.
- `npm run test` - run unit tests.
- `npm run test:ci` - run unit tests in CI mode.
- `npm run build` - produce production build output.
- `npm run reset` - reset Nx cache/workspace state.

## Formatting and Linting Policy

This workspace enforces shared formatting so all contributors use identical style rules:

- Prettier config is centralized in `.prettierrc`.
- ESLint uses the root `eslint.config.js`.
- CI runs both lint and `format:check`; formatting violations fail the pipeline.
- Pre-commit hooks run `lint-staged` so staged files are auto-formatted and linted before commit.

## Git Hooks

After `npm ci`, Husky is installed via the `prepare` script. Hooks run automatically:

- `pre-commit`: runs `lint-staged`
- `commit-msg`: validates commit format with Commitlint

## Troubleshooting

- If Nx behaves unexpectedly, run `npm run reset`.
- If API calls fail in development, verify backend is running and proxy routes are correct.
- If a commit is blocked by style checks, run `npm run format` and `npm run lint:fix`.
