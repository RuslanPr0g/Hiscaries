import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';
import { BadgeComponent } from './badge.component';

/**
 * Property 1: Badge variant always produces a CSS class
 *
 * For any `variant` value in the union `('default' | 'accent' | 'danger' | 'success')`,
 * the `Badge` component SHALL apply a non-empty CSS class corresponding to that variant —
 * no variant value is left unstyled.
 *
 * Validates: Requirements 3.2
 */
describe('BadgeComponent — Property 1: variant always produces a CSS class', () => {
    let fixture: ComponentFixture<BadgeComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [BadgeComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(BadgeComponent);
        fixture.componentRef.setInput('label', 'test');
        fixture.detectChanges();
    });

    it('should apply a non-empty variant CSS class for every value in the union', () => {
        const variantArb = fc.constantFrom(
            'default',
            'accent',
            'danger',
            'success',
        ) as fc.Arbitrary<'default' | 'accent' | 'danger' | 'success'>;

        fc.assert(
            fc.property(variantArb, (variant) => {
                fixture.componentRef.setInput('variant', variant);
                fixture.detectChanges();

                const badge: HTMLElement = fixture.nativeElement.querySelector('.badge');
                const expectedClass = `badge-${variant}`;

                // The expected class must be present and non-empty
                expect(expectedClass).not.toBe('');
                expect(badge.classList).toContain(expectedClass);

                // No other variant class should be applied simultaneously
                const allVariants: Array<'default' | 'accent' | 'danger' | 'success'> = [
                    'default',
                    'accent',
                    'danger',
                    'success',
                ];
                const otherVariants = allVariants.filter((v) => v !== variant);
                for (const other of otherVariants) {
                    expect(badge.classList).not.toContain(`badge-${other}`);
                }
            }),
        );
    });
});
