import { GlassOverlayComponent } from './glass-overlay.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';

/**
 * Property 15: GlassOverlay background-color reflects color and opacity inputs
 *
 * For any RGB string `color` and float `opacity` in [0, 1], the GlassOverlay SHALL
 * compute a `background-color` CSS value that incorporates both the `color` and
 * `opacity` values.
 *
 * Validates: Requirements 10.2
 */
describe('GlassOverlayComponent — Property 15: background-color reflects color and opacity inputs', () => {
  let fixture: ComponentFixture<GlassOverlayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GlassOverlayComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GlassOverlayComponent);
    fixture.detectChanges();
  });

  it('should compute background-color incorporating both color and opacity for any valid inputs', () => {
    fc.assert(
      fc.property(fc.tuple(fc.string(), fc.float({ min: 0, max: 1 })), ([color, opacity]) => {
        fixture.componentRef.setInput('color', color);
        fixture.componentRef.setInput('opacity', opacity);
        fixture.detectChanges();

        const component = fixture.componentInstance as unknown as {
          bgStyle: () => string;
        };
        const bgStyle = component.bgStyle();

        // The computed value must be an rgba() string
        expect(bgStyle).toMatch(/^rgba\(/);

        // It must incorporate the color string
        expect(bgStyle).toContain(color);

        // It must incorporate the opacity value
        expect(bgStyle).toContain(String(opacity));
      }),
    );
  });
});
