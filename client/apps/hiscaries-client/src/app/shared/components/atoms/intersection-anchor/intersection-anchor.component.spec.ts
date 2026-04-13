import { IntersectionAnchorComponent } from './intersection-anchor.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

// ---------------------------------------------------------------------------
// IntersectionObserver mock factory
// ---------------------------------------------------------------------------
type IntersectionCallback = (entries: IntersectionObserverEntry[]) => void;

interface MockObserverHandle {
  triggerIntersection: (isIntersecting: boolean) => void;
  disconnected: boolean;
  observerCreated: boolean;
}

function installMockIntersectionObserver(): MockObserverHandle {
  const handle: MockObserverHandle = {
    triggerIntersection: (_: boolean) => {
      // no-op until observer is created
    },
    disconnected: false,
    observerCreated: false,
  };

  class MockIO {
    constructor(callback: IntersectionCallback) {
      handle.observerCreated = true;
      handle.triggerIntersection = (isIntersecting: boolean) => {
        callback([{ isIntersecting } as IntersectionObserverEntry]);
      };
    }
    // eslint-disable-next-line @typescript-eslint/no-empty-function
    observe(_: Element): void {}
    disconnect(): void {
      handle.disconnected = true;
    }
  }

  (globalThis as unknown as Record<string, unknown>)['IntersectionObserver'] = MockIO;
  return handle;
}

function uninstallMockIntersectionObserver(original: unknown): void {
  (globalThis as unknown as Record<string, unknown>)['IntersectionObserver'] = original;
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------
describe('IntersectionAnchorComponent', () => {
  let fixture: ComponentFixture<IntersectionAnchorComponent>;
  let component: IntersectionAnchorComponent;
  let originalIO: unknown;
  let mockHandle: MockObserverHandle;

  beforeEach(async () => {
    originalIO = (globalThis as unknown as Record<string, unknown>)['IntersectionObserver'];
    mockHandle = installMockIntersectionObserver();

    await TestBed.configureTestingModule({
      imports: [IntersectionAnchorComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(IntersectionAnchorComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => {
    uninstallMockIntersectionObserver(originalIO);
  });

  // Requirements 8.1 – emits when observer fires and disabled=false
  it('should emit intersected when the element intersects and disabled is false', () => {
    fixture.componentRef.setInput('disabled', false);
    fixture.detectChanges(); // triggers ngAfterViewInit

    expect(mockHandle.observerCreated).toBe(true);

    const emitSpy = jest.spyOn(component.intersected, 'emit');

    mockHandle.triggerIntersection(true);

    expect(emitSpy).toHaveBeenCalledTimes(1);
  });

  // Requirements 8.1 – does NOT emit when isIntersecting=false
  it('should not emit intersected when isIntersecting is false', () => {
    fixture.componentRef.setInput('disabled', false);
    fixture.detectChanges();

    const emitted: void[] = [];
    component.intersected.subscribe(() => emitted.push());

    mockHandle.triggerIntersection(false);

    expect(emitted.length).toBe(0);
  });

  // Requirements 8.2 – does NOT emit when disabled=true
  it('should not emit intersected when disabled is true', () => {
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();

    const emitted: void[] = [];
    component.intersected.subscribe(() => emitted.push());

    // No observer is created when disabled=true
    mockHandle.triggerIntersection(true);

    expect(emitted.length).toBe(0);
  });

  // Requirements 8.2 – observer is not created when disabled=true
  it('should not create an IntersectionObserver when disabled is true', () => {
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();

    expect(mockHandle.observerCreated).toBe(false);
  });

  // Requirements 8.3 – observer is created when disabled is false
  it('should create an IntersectionObserver when disabled is false', () => {
    fixture.componentRef.setInput('threshold', 0.5);
    fixture.componentRef.setInput('disabled', false);
    fixture.detectChanges();

    expect(mockHandle.observerCreated).toBe(true);
  });

  // Requirements 8.4 – observer is disconnected on destroy
  it('should disconnect the IntersectionObserver on destroy', () => {
    fixture.componentRef.setInput('disabled', false);
    fixture.detectChanges();

    expect(mockHandle.disconnected).toBe(false);

    fixture.destroy();

    expect(mockHandle.disconnected).toBe(true);
  });

  // Requirements 8.5 – no error when IntersectionObserver is unavailable
  it('should log a warning and not throw when IntersectionObserver is unavailable', () => {
    // Remove IntersectionObserver from global scope
    delete (globalThis as unknown as Record<string, unknown>)['IntersectionObserver'];

    const warnSpy = jest.spyOn(console, 'warn').mockImplementation(jest.fn());

    expect(() => {
      fixture.componentRef.setInput('disabled', false);
      fixture.detectChanges();
    }).not.toThrow();

    expect(warnSpy).toHaveBeenCalled();
    warnSpy.mockRestore();
  });
});
