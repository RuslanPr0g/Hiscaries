---
paths:
  - "client/**/*.spec.ts"
---

# Frontend Unit Testing Conventions

- Runner: **Jest** via `ts-jest` ESM transformer (`client/jest.config.ts`, `client/jest.preset.js`). Run with `nx test hiscaries-client`, or a single file with `nx test hiscaries-client --testFile=path/to/x.spec.ts`.
- Component tests use Angular's own `TestBed` + `ComponentFixture` — **not** Angular Testing Library and **not** `ng-mocks`. Configure with `TestBed.configureTestingModule({ imports: [StandaloneComponent] })` (standalone components go in `imports`, never `declarations`), create the fixture, set inputs via `fixture.componentRef.setInput(...)`, and call `fixture.detectChanges()` before assertions.
- Two co-located spec shapes exist per component, both valid and often both present:
  - `*.component.spec.ts` — example-based tests asserting concrete input → rendered-output behavior (e.g. `badge.component.spec.ts`).
  - `*.property.spec.ts` — property-based tests using `fast-check` (`fc.assert(fc.property(arb, (value) => {...}))`) to check an invariant holds across a generated input domain (e.g. `badge.property.spec.ts`). Use this shape when the property to verify is naturally universal (e.g. "every variant produces exactly one class"), not as a replacement for example-based tests.
- Query the DOM via `fixture.nativeElement.querySelector(...)`, not by fixture debugElement queries or test-id libraries, to match existing specs.
- Do not introduce Angular Testing Library, `ng-mocks`, or any second testing paradigm without discussing it first — the existing 34+ spec files all follow the TestBed + optional fast-check pattern above, and mixing paradigms risks silent drift.
