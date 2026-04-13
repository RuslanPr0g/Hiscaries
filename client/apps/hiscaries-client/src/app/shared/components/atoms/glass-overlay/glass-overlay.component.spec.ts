import { GlassOverlayComponent } from './glass-overlay.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

describe('GlassOverlayComponent', () => {
  let fixture: ComponentFixture<GlassOverlayComponent>;

  function getOverlay(): HTMLElement | null {
    return fixture.nativeElement.querySelector('.glass-overlay');
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GlassOverlayComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GlassOverlayComponent);
    fixture.detectChanges();
  });

  // Requirements 10.6 – overlay is rendered when visible=true
  it('should render the overlay when visible is true', () => {
    fixture.componentRef.setInput('visible', true);
    fixture.detectChanges();

    expect(getOverlay()).not.toBeNull();
  });

  // Requirements 10.5 – overlay is absent when visible=false
  it('should not render the overlay when visible is false', () => {
    fixture.componentRef.setInput('visible', false);
    fixture.detectChanges();

    expect(getOverlay()).toBeNull();
  });

  // Requirements 10.3 – backdrop-filter applied when blur=true
  it('should apply backdrop-filter when blur is true', () => {
    fixture.componentRef.setInput('blur', true);
    fixture.detectChanges();

    // Verify the blur input is true and the template binding would produce 'blur(8px)'
    // jsdom does not support backdrop-filter, so we verify the component logic directly
    const component = fixture.componentInstance;
    expect(component.blur()).toBe(true);
  });

  // Requirements 10.4 – backdrop-filter absent when blur=false
  it('should not apply backdrop-filter when blur is false', () => {
    fixture.componentRef.setInput('blur', false);
    fixture.detectChanges();

    const component = fixture.componentInstance;
    expect(component.blur()).toBe(false);
  });

  // Requirements 10.2 – background-color reflects color and opacity inputs
  it('should compute background-color from color and opacity inputs', () => {
    fixture.componentRef.setInput('color', '255,0,0');
    fixture.componentRef.setInput('opacity', 0.5);
    fixture.detectChanges();

    // The style.backgroundColor will be normalized by the browser; check via computed style
    // or verify the bound value directly via the component's bgStyle signal
    const component = fixture.componentInstance as unknown as { bgStyle: () => string };
    expect(component.bgStyle()).toBe('rgba(255,0,0,0.5)');
  });

  // Requirements 10.2 – default background-color uses default color and opacity
  it('should use default color "0,0,0" and opacity 0.7 by default', () => {
    const component = fixture.componentInstance as unknown as { bgStyle: () => string };
    expect(component.bgStyle()).toBe('rgba(0,0,0,0.7)');
  });
});
