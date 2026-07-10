---
paths:
  - "client/apps/hiscaries-client/src/app/**/*.ts"
---

# Angular Conventions

- All components are **standalone** — no `NgModule`s. Import dependencies directly in the component's `imports` array.
- Use `ChangeDetectionStrategy.OnPush` on new components unless there's a specific reason not to (e.g. a component that must react to mutation-based third-party state outside Angular's change detection).
- Dependency injection uses `inject()` at field-initializer position (`private http = inject(HttpClient);`), not constructor-parameter injection.
- Reactive local/shared state prefers `signal()` / `computed()` over manually-managed fields + `ChangeDetectorRef.markForCheck()`. See `frontend/ngrx.md` for when to use a signals-based service vs. the one existing NgRx store.
- Cross-folder imports use the path aliases from `client/tsconfig.base.json` — `@stories/*`, `@shared/*`, `@media/*`, `@users/*`, `@user-to-story/*`, `@admin/*`, `@environments/*` — never a relative path (`../../../`) that crosses one of these domain folders. Relative imports are fine within the same feature folder.
- Layout follows atomic design under `shared/components/`: `atoms/` (e.g. `badge`, `glass-card`) for the smallest reusable pieces, `molecules/` (e.g. `status-banner`, `media-card`) for compositions of atoms. Place new shared UI in the tier matching its composition, not by guessing — an atom has no sub-components other than native elements; a molecule composes atoms.
- Styling is SCSS, co-located per component (`x.component.scss` next to `x.component.ts`) — no global utility classes.
- TypeScript strict mode is on; don't silence type errors with `any` or non-null assertions where a narrower type or a guard is feasible.
