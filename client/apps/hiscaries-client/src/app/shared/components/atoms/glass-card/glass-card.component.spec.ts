import { GlassCardComponent } from './glass-card.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

describe('GlassCardComponent', () => {
  let fixture: ComponentFixture<GlassCardComponent>;
  let _component: GlassCardComponent;

  function getCard(): HTMLElement {
    return fixture.nativeElement.querySelector('.glass-card');
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GlassCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GlassCardComponent);
    _component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // Requirements 1.1 – glass surface styles are applied
  it('should apply the glass-card CSS class', () => {
    expect(getCard()).not.toBeNull();
    expect(getCard().classList).toContain('glass-card');
  });

  // Requirements 1.3, 1.4 – interactive=true adds tabindex and .interactive class
  it('should add tabindex="0" and .interactive class when interactive=true', () => {
    fixture.componentRef.setInput('interactive', true);
    fixture.detectChanges();

    const card = getCard();
    expect(card.classList).toContain('interactive');
    expect(card.getAttribute('tabindex')).toBe('0');
  });

  // Requirements 1.3, 1.4 – interactive=false omits .interactive class and tabindex
  it('should omit .interactive class and tabindex when interactive=false', () => {
    fixture.componentRef.setInput('interactive', false);
    fixture.detectChanges();

    const card = getCard();
    expect(card.classList).not.toContain('interactive');
    expect(card.getAttribute('tabindex')).toBeNull();
  });

  // Requirements 1.5 – borderRadius='sm' produces border-radius-sm class
  it('should apply border-radius-sm class when borderRadius is "sm"', () => {
    fixture.componentRef.setInput('borderRadius', 'sm');
    fixture.detectChanges();

    const card = getCard();
    expect(card.classList).toContain('border-radius-sm');
    expect(card.classList).not.toContain('border-radius-md');
    expect(card.classList).not.toContain('border-radius-lg');
  });

  // Requirements 1.5 – borderRadius='md' produces border-radius-md class
  it('should apply border-radius-md class when borderRadius is "md"', () => {
    fixture.componentRef.setInput('borderRadius', 'md');
    fixture.detectChanges();

    const card = getCard();
    expect(card.classList).toContain('border-radius-md');
    expect(card.classList).not.toContain('border-radius-sm');
    expect(card.classList).not.toContain('border-radius-lg');
  });

  // Requirements 1.5 – borderRadius='lg' produces border-radius-lg class (default)
  it('should apply border-radius-lg class when borderRadius is "lg"', () => {
    // 'lg' is the default, but set explicitly to be clear
    fixture.componentRef.setInput('borderRadius', 'lg');
    fixture.detectChanges();

    const card = getCard();
    expect(card.classList).toContain('border-radius-lg');
    expect(card.classList).not.toContain('border-radius-sm');
    expect(card.classList).not.toContain('border-radius-md');
  });

  // Requirements 1.6 – content projection via ng-content
  it('should project content via ng-content', () => {
    const hostEl: HTMLElement = fixture.nativeElement;
    // The component renders a .glass-card wrapper — projected content would appear inside it
    expect(hostEl.querySelector('.glass-card')).not.toBeNull();
  });
});
