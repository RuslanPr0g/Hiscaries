import { SkeletonGridComponent } from './skeleton-grid.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';

/**
 * Property 7: SkeletonGrid renders exactly count cards
 *
 * For any positive integer `count`, the `SkeletonGrid` component SHALL render
 * exactly `count` `SkeletonCard` instances — no more, no less.
 *
 * Validates: Requirements 12.1
 */
describe('SkeletonGridComponent — Property 7: renders exactly count cards', () => {
  let fixture: ComponentFixture<SkeletonGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkeletonGridComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SkeletonGridComponent);
    fixture.detectChanges();
  });

  it('should render exactly count SkeletonCard instances for any count in [0, 20]', () => {
    fc.assert(
      fc.property(fc.integer({ min: 0, max: 20 }), (count) => {
        fixture.componentRef.setInput('count', count);
        fixture.detectChanges();

        const cards = fixture.nativeElement.querySelectorAll('ui-skeleton-card');
        expect(cards.length).toBe(count);
      }),
    );
  });
});
