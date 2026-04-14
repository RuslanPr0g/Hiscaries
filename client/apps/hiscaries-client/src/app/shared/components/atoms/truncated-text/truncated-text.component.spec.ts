import { TruncatedTextComponent } from './truncated-text.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

describe('TruncatedTextComponent', () => {
  let fixture: ComponentFixture<TruncatedTextComponent>;

  function getText(): string {
    return fixture.nativeElement.textContent?.trim() ?? '';
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TruncatedTextComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TruncatedTextComponent);
    fixture.componentRef.setInput('text', 'Hello');
    fixture.detectChanges();
  });

  // Requirements 9.1 — truncation when text exceeds maxLength
  it('should truncate text and append suffix when text exceeds maxLength', () => {
    fixture.componentRef.setInput('text', 'Hello World');
    fixture.componentRef.setInput('maxLength', 5);
    fixture.detectChanges();

    expect(getText()).toBe('Hello...');
  });

  // Requirements 9.1 — custom suffix is appended after truncation
  it('should use a custom suffix when provided', () => {
    fixture.componentRef.setInput('text', 'Hello World');
    fixture.componentRef.setInput('maxLength', 5);
    fixture.componentRef.setInput('suffix', ' [more]');
    fixture.detectChanges();

    expect(getText()).toBe('Hello [more]');
  });

  // Requirements 9.2 — full text when text length equals maxLength
  it('should render full text when text length equals maxLength', () => {
    fixture.componentRef.setInput('text', 'Hello');
    fixture.componentRef.setInput('maxLength', 5);
    fixture.detectChanges();

    expect(getText()).toBe('Hello');
  });

  // Requirements 9.2 — full text when text length is less than maxLength
  it('should render full text when text length is less than maxLength', () => {
    fixture.componentRef.setInput('text', 'Hi');
    fixture.componentRef.setInput('maxLength', 10);
    fixture.detectChanges();

    expect(getText()).toBe('Hi');
  });

  // Requirements 9.3 — empty string renders empty without suffix
  it('should render empty string without suffix when text is empty', () => {
    fixture.componentRef.setInput('text', '');
    fixture.componentRef.setInput('maxLength', 5);
    fixture.detectChanges();

    expect(getText()).toBe('');
  });

  // Requirements 9.4 — tag 'span' renders a <span> element
  it('should render inside a <span> when tag is "span"', () => {
    fixture.componentRef.setInput('tag', 'span');
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('span')).not.toBeNull();
  });

  // Requirements 9.4 — tag 'p' renders a <p> element
  it('should render inside a <p> when tag is "p"', () => {
    fixture.componentRef.setInput('tag', 'p');
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('p')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('span')).toBeNull();
  });

  // Requirements 9.4 — tag 'h1' renders an <h1> element
  it('should render inside an <h1> when tag is "h1"', () => {
    fixture.componentRef.setInput('tag', 'h1');
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('h1')).not.toBeNull();
  });

  // Requirements 9.4 — tag 'h2' renders an <h2> element
  it('should render inside an <h2> when tag is "h2"', () => {
    fixture.componentRef.setInput('tag', 'h2');
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('h2')).not.toBeNull();
  });

  // Requirements 9.4 — tag 'h3' renders an <h3> element
  it('should render inside an <h3> when tag is "h3"', () => {
    fixture.componentRef.setInput('tag', 'h3');
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('h3')).not.toBeNull();
  });

  // Requirements 9.4 — tag 'h4' renders an <h4> element
  it('should render inside an <h4> when tag is "h4"', () => {
    fixture.componentRef.setInput('tag', 'h4');
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('h4')).not.toBeNull();
  });

  // Requirements 9.4 — default tag is 'span'
  it('should default to <span> when no tag is provided', () => {
    // No tag set — relies on default input value
    expect(fixture.nativeElement.querySelector('span')).not.toBeNull();
  });
});
