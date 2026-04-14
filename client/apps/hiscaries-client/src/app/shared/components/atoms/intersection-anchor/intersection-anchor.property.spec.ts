import { IntersectionAnchorComponent } from './intersection-anchor.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';

/**
 * Property 9: IntersectionAnchor does not emit when disabled
 *
 * For any viewport intersection event, if `IntersectionAnchor.disabled` is `true`,
 * the `intersected` output SHALL not emit.
 *
 * Validates: Requirements 8.2
 */

// ---------------------------------------------------------------------------
// IntersectionObserver mock factory
// ---------------------------------------------------------------------------
type IntersectionCallback = (entries: IntersectionObserverEntry[]) => void;

interface MockObserverHandle {
  triggerIntersection: (isIntersecting: boolean) => void;
  observerCreated: boolean;
}

function installMockIntersectionObserver(): MockObserverHandle {
  const handle: MockObserverHandle = {
    triggerIntersection: (_isIntersecting: boolean) => {
      // no-op until observer is created
    },
    observerCreated: false,
  };

  class MockIO {
    constructor(callback: IntersectionCallback) {
      handle.observerCreated = true;
      handle.triggerIntersection = (isIntersecting: boolean) => {
        callback([{ isIntersecting } as IntersectionObserverEntry]);
      };
    }
    observe(_el: Element): void {
      /* noop */
    }
    disconnect(): void {
      /* noop */
    }
  }

  (globalThis as unknown as Record<string, unknown>)['IntersectionObserver'] = MockIO;
  return handle;
}

// ---------------------------------------------------------------------------
// Property test
// ---------------------------------------------------------------------------
describe('IntersectionAnchorComponent — Property 9: does not emit when disabled', () => {
  let fixture: ComponentFixture<IntersectionAnchorComponent>;
  let originalIO: unknown;

  beforeEach(async () => {
    originalIO = (globalThis as unknown as Record<string, unknown>)['IntersectionObserver'];

    await TestBed.configureTestingModule({
      imports: [IntersectionAnchorComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(IntersectionAnchorComponent);
  });

  afterEach(() => {
    (globalThis as unknown as Record<string, unknown>)['IntersectionObserver'] = originalIO;
    fixture.destroy();
  });

  it('should never emit intersected when disabled=true, for any intersection event', () => {
    fc.assert(
      fc.property(
        fc.boolean(), // isIntersecting value
        (isIntersecting) => {
          // Install a fresh mock for each run
          const mockHandle = installMockIntersectionObserver();

          // Subscribe before detectChanges so we catch any emissions
          const emitted: void[] = [];
          fixture.componentInstance.intersected.subscribe(() => emitted.push());

          fixture.componentRef.setInput('disabled', true);
          fixture.detectChanges();

          // Simulate an intersection event (even if no observer was created)
          mockHandle.triggerIntersection(isIntersecting);

          expect(emitted.length).toBe(0);
        },
      ),
    );
  });
});
