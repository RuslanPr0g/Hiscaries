import { SectionHeaderComponent } from './section-header.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';

/**
 * Property 11: SectionHeader renders correct heading tag
 *
 * For any `level` value in `(1 | 2 | 3)`, the `SectionHeader` component SHALL render
 * the output inside the corresponding `<h1>`, `<h2>`, or `<h3>` element.
 *
 * Validates: Requirements 14.1
 */
describe('SectionHeaderComponent — Property 11: correct heading tag', () => {
  let fixture: ComponentFixture<SectionHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SectionHeaderComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SectionHeaderComponent);
    fixture.componentRef.setInput('title', 'Test');
    fixture.detectChanges();
  });

  it('should render the correct heading element for every level in (1 | 2 | 3)', () => {
    const levelArb = fc.constantFrom(1, 2, 3) as fc.Arbitrary<1 | 2 | 3>;

    fc.assert(
      fc.property(levelArb, (level) => {
        fixture.componentRef.setInput('level', level);
        fixture.detectChanges();

        const expectedTag = `h${level}`;
        const heading: HTMLElement = fixture.nativeElement.querySelector(expectedTag);

        // The correct heading element must be present
        expect(heading).not.toBeNull();

        // No other heading elements should be rendered simultaneously
        const allTags = ['h1', 'h2', 'h3'].filter((t) => t !== expectedTag);
        for (const tag of allTags) {
          expect(fixture.nativeElement.querySelector(tag)).toBeNull();
        }
      }),
    );
  });
});
