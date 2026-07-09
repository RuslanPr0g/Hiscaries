# Client — Claude Context

All commands run from the `client/` directory.

## Nx Commands

| Command                                               | What it does                         |
| ----------------------------------------------------- | ------------------------------------ |
| `nx serve hiscaries-client`                           | Start Angular dev server             |
| `nx build hiscaries-client`                           | Production build                     |
| `nx test hiscaries-client`                            | Run all tests                        |
| `nx test hiscaries-client --testFile=path/to/spec.ts` | Run a single test file               |
| `nx lint hiscaries-client`                            | Lint                                 |
| `nx lint hiscaries-client --fix`                      | Lint and auto-fix                    |
| `nx affected:test --base=master`                      | Test only projects changed vs master |

## Angular Conventions

- All components are **standalone** — no NgModules
- State: **NgRx store + effects** for any shared/async state; no component-local state for data that
  crosses components
- Styling: **SCSS per-component**, no global utility classes
- TypeScript: **strict mode**, target ES2022 (`client/tsconfig.base.json`)

## Path Aliases

Defined in `client/tsconfig.base.json`:

| Alias              | Resolves to                                     |
| ------------------ | ----------------------------------------------- |
| `@admin/*`         | `apps/hiscaries-client/src/app/admin/*`         |
| `@shared/*`        | `apps/hiscaries-client/src/app/shared/*`        |
| `@stories/*`       | `apps/hiscaries-client/src/app/stories/*`       |
| `@media/*`         | `apps/hiscaries-client/src/app/media/*`         |
| `@users/*`         | `apps/hiscaries-client/src/app/users/*`         |
| `@user-to-story/*` | `apps/hiscaries-client/src/app/user-to-story/*` |
| `@environments/*`  | `apps/hiscaries-client/src/environments/*`      |

## Testing

- Runner: **Jest** with **ts-jest ESM** transformer (`client/jest.config.ts`)
- `transformIgnorePatterns` is customised for `@autofiy/*` packages — do not change
- Preset: `client/jest.preset.js`

## Linting

- Config: `client/eslint.config.js` (flat config format)
- Run via `nx lint hiscaries-client` — do not call `eslint` directly
