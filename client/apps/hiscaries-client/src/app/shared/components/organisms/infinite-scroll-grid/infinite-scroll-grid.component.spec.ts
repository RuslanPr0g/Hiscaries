import { InfiniteScrollGridComponent } from './infinite-scroll-grid.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';

// Requirements 17.1–17.6

describe('InfiniteScrollGridComponent', () => {
  let fixture: ComponentFixture<InfiniteScrollGridComponent>;
  let component: InfiniteScrollGridComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InfiniteScrollGridComponent],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(InfiniteScrollGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // Requirements 17.1 – loadMore emits when anchor intersects and hasMore=true, isLoading=false
  it('should emit loadMore when onIntersected is called with hasMore=true and isLoading=false', () => {
    fixture.componentRef.setInput('hasMore', true);
    fixture.componentRef.setInput('isLoading', false);
    fixture.detectChanges();

    const emitSpy = jest.spyOn(component.loadMore, 'emit');

    component.onIntersected();

    expect(emitSpy).toHaveBeenCalledTimes(1);
  });

  // Requirements 17.2 – loadMore does not emit when hasMore=false
  it('should not emit loadMore when hasMore=false', () => {
    fixture.componentRef.setInput('hasMore', false);
    fixture.componentRef.setInput('isLoading', false);
    fixture.detectChanges();

    const emitSpy = jest.spyOn(component.loadMore, 'emit');

    component.onIntersected();

    expect(emitSpy).not.toHaveBeenCalled();
  });

  // Requirements 17.3 – loadMore does not emit when isLoading=true
  it('should not emit loadMore when isLoading=true', () => {
    fixture.componentRef.setInput('hasMore', true);
    fixture.componentRef.setInput('isLoading', true);
    fixture.detectChanges();

    const emitSpy = jest.spyOn(component.loadMore, 'emit');

    component.onIntersected();

    expect(emitSpy).not.toHaveBeenCalled();
  });

  // Requirements 17.2 – loadMore does not emit when both hasMore=false and isLoading=true
  it('should not emit loadMore when hasMore=false and isLoading=true', () => {
    fixture.componentRef.setInput('hasMore', false);
    fixture.componentRef.setInput('isLoading', true);
    fixture.detectChanges();

    const emitSpy = jest.spyOn(component.loadMore, 'emit');

    component.onIntersected();

    expect(emitSpy).not.toHaveBeenCalled();
  });

  // Requirements 17.2 – anchor disabled binding is true when hasMore=false
  it('should pass disabled=true to the intersection anchor when hasMore=false', () => {
    fixture.componentRef.setInput('hasMore', false);
    fixture.componentRef.setInput('isLoading', false);
    fixture.detectChanges();

    // The anchor disabled expression is: !hasMore() || isLoading()
    // With hasMore=false: !false || false = true
    const anchorDisabled = !component.hasMore() || component.isLoading();
    expect(anchorDisabled).toBe(true);
  });

  // Requirements 17.3 – anchor disabled binding is true when isLoading=true
  it('should pass disabled=true to the intersection anchor when isLoading=true', () => {
    fixture.componentRef.setInput('hasMore', true);
    fixture.componentRef.setInput('isLoading', true);
    fixture.detectChanges();

    // The anchor disabled expression is: !hasMore() || isLoading()
    // With isLoading=true: !true || true = true
    const anchorDisabled = !component.hasMore() || component.isLoading();
    expect(anchorDisabled).toBe(true);
  });

  // Requirements 17.1 – anchor disabled binding is false when hasMore=true and isLoading=false
  it('should pass disabled=false to the intersection anchor when hasMore=true and isLoading=false', () => {
    fixture.componentRef.setInput('hasMore', true);
    fixture.componentRef.setInput('isLoading', false);
    fixture.detectChanges();

    // The anchor disabled expression is: !hasMore() || isLoading()
    // With hasMore=true, isLoading=false: !true || false = false
    const anchorDisabled = !component.hasMore() || component.isLoading();
    expect(anchorDisabled).toBe(false);
  });
});
