---
paths:
  - "client/apps/hiscaries-client/src/app/**/store/**/*.ts"
---

# NgRx / State Management Conventions

**This diverges from `client/CLAUDE.md`**, which currently states "NgRx store + effects for any shared/async state." In actual practice, NgRx is used in exactly one place in this codebase: `stories/store/` (`story.actions.ts`, `story.reducer.ts`, `story.selector.ts`, `story-state.model.ts`, `story-pdf.effects.ts`), scoped to the stories search/PDF-save flow. Reconciling the `client/CLAUDE.md` line with actual code is tracked as a follow-up, not resolved by this rule — but until it is, this rule reflects what to actually do:

- **Default to signals + a stateful `@Injectable({ providedIn: 'root' })` service** for new shared/async state (the pattern used in `users/services/*.service.ts`, `stories/services/*.service.ts`, etc.): inject dependencies with `inject()`, expose reactive state via `signal()`/`computed()`, and keep HTTP calls returning `Observable` from a thin service method rather than dispatching NgRx actions.
- **Only extend the existing NgRx store** if you're adding to the stories search/PDF-save flow it already owns. Don't create a second NgRx store for a new domain (media, notifications, user-to-story, etc.) — use the signals + service pattern there instead.
- If a file under `stories/store/**` needs a new action, add it to `story.actions.ts` following the existing action-group naming, handle it in `story.reducer.ts`, and add/extend selectors in `story.selector.ts` rather than deriving state ad hoc in components. Side effects (HTTP, PDF generation) go in `story-pdf.effects.ts` or a sibling `*.effects.ts` file, not in the reducer.
- Components consume store state via `Store.select(selector)` and dispatch via `Store.dispatch(action)` — don't reach into the reducer or feature state shape directly from a component.
