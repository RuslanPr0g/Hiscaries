import { TruncatedTextComponent } from './truncated-text.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';

// Helper: pure truncation logic mirroring the component's computed()
function truncate(text: string, maxLength: number, suffix: string): string {
  return text.length <= maxLength ? text : text.slice(0, maxLength) + suffix;
}

describe('TruncatedTextComponent — Property Tests', () => {
  let fixture: ComponentFixture<TruncatedTextComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TruncatedTextComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TruncatedTextComponent);
    fixture.componentRef.setInput('text', 'seed');
    fixture.detectChanges();
  });

  /**
   * Property 3: TruncatedText output length invariant
   *
   * For any non-empty string `s` and positive integer `n`, the rendered text
   * produced by TruncatedText SHALL have length ≤ `n + suffix.length`.
   *
   * Validates: Requirements 9.1
   */
  it('Property 3 — rendered text length is always ≤ maxLength + suffix.length', () => {
    fc.assert(
      fc.property(
        fc.string({ minLength: 1 }),
        fc.integer({ min: 1, max: 200 }),
        fc.string(),
        (s, n, suffix) => {
          fixture.componentRef.setInput('text', s);
          fixture.componentRef.setInput('maxLength', n);
          fixture.componentRef.setInput('suffix', suffix);
          fixture.detectChanges();

          const rendered: string = fixture.nativeElement.textContent ?? '';
          expect(rendered.length).toBeLessThanOrEqual(n + suffix.length);
        },
      ),
    );
  });

  /**
   * Property 4: TruncatedText identity when short
   *
   * For any string `s` where `s.length ≤ maxLength`, the rendered text
   * produced by TruncatedText SHALL equal `s` exactly (no suffix appended).
   *
   * Validates: Requirements 9.2
   */
  it('Property 4 — renders full text unchanged when text length ≤ maxLength', () => {
    fc.assert(
      fc.property(
        fc
          .integer({ min: 0, max: 200 })
          .chain((maxLength) => fc.tuple(fc.string({ maxLength }), fc.constant(maxLength))),
        ([s, n]) => {
          fixture.componentRef.setInput('text', s);
          fixture.componentRef.setInput('maxLength', n);
          fixture.detectChanges();

          const rendered: string = fixture.nativeElement.textContent ?? '';
          expect(rendered).toBe(s);
        },
      ),
    );
  });

  /**
   * Property 5: TruncatedText truncation is idempotent
   *
   * For any string `s` and positive integer `n`, applying truncation twice
   * with the same `n` SHALL produce the same output as applying it once —
   * i.e., truncate(truncate(s, n), n) === truncate(s, n).
   *
   * Validates: Requirements 9.1, 9.2
   */
  it('Property 5 — truncation is idempotent', () => {
    fc.assert(
      fc.property(fc.string(), fc.integer({ min: 1, max: 200 }), fc.string(), (s, n, suffix) => {
        const once = truncate(s, n, suffix);
        const twice = truncate(once, n, suffix);
        expect(twice).toBe(once);
      }),
    );
  });

  /**
   * Property 6: TruncatedText tag renders correct HTML element
   *
   * For any `tag` value in the union `('p' | 'h1' | 'h2' | 'h3' | 'h4' | 'span')`,
   * the TruncatedText component SHALL render the output inside an element of that exact tag type.
   *
   * Validates: Requirements 9.4
   */
  it('Property 6 — renders inside the correct HTML element for every tag value', () => {
    const tagArb = fc.constantFrom('p', 'h1', 'h2', 'h3', 'h4', 'span') as fc.Arbitrary<
      'p' | 'h1' | 'h2' | 'h3' | 'h4' | 'span'
    >;

    fc.assert(
      fc.property(tagArb, fc.string(), (tag, text) => {
        fixture.componentRef.setInput('tag', tag);
        fixture.componentRef.setInput('text', text);
        fixture.detectChanges();

        const el: HTMLElement | null = fixture.nativeElement.querySelector(tag);
        expect(el).not.toBeNull();
      }),
    );
  });
});
