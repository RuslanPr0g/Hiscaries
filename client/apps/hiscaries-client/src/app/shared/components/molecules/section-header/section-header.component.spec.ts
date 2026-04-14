import { SectionHeaderComponent } from './section-header.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

// Requirements 14.1–14.3

describe('SectionHeaderComponent', () => {
  let fixture: ComponentFixture<SectionHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SectionHeaderComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SectionHeaderComponent);
    fixture.componentRef.setInput('title', 'Test Title');
    fixture.detectChanges();
  });

  // Requirements 14.1 – level 1 renders <h1>
  it('should render an <h1> when level is 1', () => {
    fixture.componentRef.setInput('level', 1);
    fixture.detectChanges();

    const h1 = fixture.nativeElement.querySelector('h1');
    expect(h1).not.toBeNull();
    expect(h1.textContent.trim()).toBe('Test Title');
  });

  // Requirements 14.1 – level 2 renders <h2>
  it('should render an <h2> when level is 2', () => {
    fixture.componentRef.setInput('level', 2);
    fixture.detectChanges();

    const h2 = fixture.nativeElement.querySelector('h2');
    expect(h2).not.toBeNull();
    expect(h2.textContent.trim()).toBe('Test Title');
  });

  // Requirements 14.1 – level 3 renders <h3>
  it('should render an <h3> when level is 3', () => {
    fixture.componentRef.setInput('level', 3);
    fixture.detectChanges();

    const h3 = fixture.nativeElement.querySelector('h3');
    expect(h3).not.toBeNull();
    expect(h3.textContent.trim()).toBe('Test Title');
  });

  // Requirements 14.1 – default level (1) renders <h1>
  it('should default to <h1> when no level is provided', () => {
    const h1 = fixture.nativeElement.querySelector('h1');
    expect(h1).not.toBeNull();
  });

  // Requirements 14.2 – main-title class is applied for level 1
  it('should apply the main-title class to the h1 element', () => {
    fixture.componentRef.setInput('level', 1);
    fixture.detectChanges();

    const h1 = fixture.nativeElement.querySelector('h1');
    expect(h1.classList).toContain('main-title');
  });

  // Requirements 14.2 – main-title class is applied for level 2
  it('should apply the main-title class to the h2 element', () => {
    fixture.componentRef.setInput('level', 2);
    fixture.detectChanges();

    const h2 = fixture.nativeElement.querySelector('h2');
    expect(h2.classList).toContain('main-title');
  });

  // Requirements 14.2 – main-title class is applied for level 3
  it('should apply the main-title class to the h3 element', () => {
    fixture.componentRef.setInput('level', 3);
    fixture.detectChanges();

    const h3 = fixture.nativeElement.querySelector('h3');
    expect(h3.classList).toContain('main-title');
  });

  // Requirements 14.1 – only one heading element is rendered at a time
  it('should render only one heading element', () => {
    fixture.componentRef.setInput('level', 2);
    fixture.detectChanges();

    const headings = fixture.nativeElement.querySelectorAll('h1, h2, h3');
    expect(headings.length).toBe(1);
  });
});
