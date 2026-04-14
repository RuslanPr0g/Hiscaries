import { StatusBannerComponent } from './status-banner.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';

/**
 * Property 12: StatusBanner type always produces a color style
 *
 * For any `type` value in the union `('info' | 'warning' | 'error' | 'success')`,
 * the `StatusBanner` component SHALL apply a non-empty color style corresponding
 * to that type — no type value is left unstyled.
 *
 * Validates: Requirements 15.2
 */
describe('StatusBannerComponent — Property 12: type always produces a color style', () => {
  let fixture: ComponentFixture<StatusBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusBannerComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(StatusBannerComponent);
    fixture.componentRef.setInput('message', 'Test');
    fixture.detectChanges();
  });

  it('should apply a non-empty banner color class for every type in the union', () => {
    const typeArb = fc.constantFrom('info', 'warning', 'error', 'success') as fc.Arbitrary<
      'info' | 'warning' | 'error' | 'success'
    >;

    fc.assert(
      fc.property(typeArb, (type) => {
        fixture.componentRef.setInput('type', type);
        fixture.detectChanges();

        const banner: HTMLElement = fixture.nativeElement.querySelector('.status-banner');

        // The expected color class must be present and non-empty
        const expectedClass = `banner-${type}`;
        expect(banner.classList).toContain(expectedClass);

        // The class string itself must be non-empty
        expect(expectedClass.length).toBeGreaterThan(0);
      }),
    );
  });
});
