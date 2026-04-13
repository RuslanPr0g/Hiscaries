import { InfiniteScrollGridComponent } from './infinite-scroll-grid.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';

/**
 * Property 10: InfiniteScrollGrid does not emit loadMore when hasMore=false or isLoading=true
 *
 * For any intersection event, if InfiniteScrollGrid.hasMore is false OR
 * InfiniteScrollGrid.isLoading is true, the loadMore output SHALL not emit.
 *
 * Validates: Requirements 17.2, 17.3
 */
describe('InfiniteScrollGridComponent — Property 10: does not emit loadMore when hasMore=false or isLoading=true', () => {
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

  it('should never emit loadMore when hasMore=false OR isLoading=true', () => {
    // Generator: pairs where at least one of (hasMore=false, isLoading=true) holds
    const suppressedArb = fc.oneof(
      // hasMore=false, isLoading=any
      fc.record({ hasMore: fc.constant(false), isLoading: fc.boolean() }),
      // hasMore=any, isLoading=true
      fc.record({ hasMore: fc.boolean(), isLoading: fc.constant(true) }),
    );

    fc.assert(
      fc.property(suppressedArb, ({ hasMore, isLoading }) => {
        fixture.componentRef.setInput('hasMore', hasMore);
        fixture.componentRef.setInput('isLoading', isLoading);
        fixture.detectChanges();

        const emitSpy = jest.spyOn(component.loadMore, 'emit');

        component.onIntersected();

        expect(emitSpy).not.toHaveBeenCalled();

        emitSpy.mockRestore();
      }),
    );
  });
});
