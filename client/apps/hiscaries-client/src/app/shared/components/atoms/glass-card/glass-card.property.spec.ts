import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';
import { GlassCardComponent } from './glass-card.component';

/**
 * Property 13: GlassCard borderRadius always produces a CSS class
 *
 * For any `borderRadius` value in the union `('sm' | 'md' | 'lg')`, the
 * `GlassCard` component SHALL apply a non-empty CSS class corresponding to
 * that border-radius value.
 *
 * Validates: Requirements 1.5
 */
describe('GlassCardComponent — Property 13: borderRadius always produces a CSS class', () => {
    let fixture: ComponentFixture<GlassCardComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [GlassCardComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(GlassCardComponent);
        fixture.detectChanges();
    });

    it('should apply a non-empty border-radius CSS class for every value in the union', () => {
        const borderRadiusArb = fc.constantFrom('sm', 'md', 'lg') as fc.Arbitrary<'sm' | 'md' | 'lg'>;

        fc.assert(
            fc.property(borderRadiusArb, (borderRadius) => {
                fixture.componentRef.setInput('borderRadius', borderRadius);
                fixture.detectChanges();

                const card: HTMLElement = fixture.nativeElement.querySelector('.glass-card');
                const expectedClass = `border-radius-${borderRadius}`;

                // The expected class must be present and non-empty
                expect(expectedClass).not.toBe('');
                expect(card.classList).toContain(expectedClass);

                // No other border-radius class should be applied simultaneously
                const allRadiusValues: Array<'sm' | 'md' | 'lg'> = ['sm', 'md', 'lg'];
                const otherValues = allRadiusValues.filter((v) => v !== borderRadius);
                for (const other of otherValues) {
                    expect(card.classList).not.toContain(`border-radius-${other}`);
                }
            }),
        );
    });
});
