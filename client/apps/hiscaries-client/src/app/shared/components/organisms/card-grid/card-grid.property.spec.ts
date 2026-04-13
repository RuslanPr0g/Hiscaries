import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';
import { CardGridComponent } from './card-grid.component';

/**
 * Property 8: CardGrid mutual exclusion of render states
 *
 * For any combination of inputs (isLoading, isEmpty, errorTitle), the CardGrid
 * component SHALL render exactly one of skeleton, error, empty, or content —
 * never zero, never two simultaneously.
 *
 * Validates: Requirements 16.5
 */
describe('CardGridComponent — Property 8: mutual exclusion of render states', () => {
    let fixture: ComponentFixture<CardGridComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CardGridComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CardGridComponent);
        fixture.detectChanges();
    });

    it('should render exactly one state for any combination of isLoading, isEmpty, errorTitle', () => {
        fc.assert(
            fc.property(
                fc.record({
                    isLoading: fc.boolean(),
                    isEmpty: fc.boolean(),
                    errorTitle: fc.option(fc.string(), { nil: null }),
                }),
                ({ isLoading, isEmpty, errorTitle }) => {
                    fixture.componentRef.setInput('isLoading', isLoading);
                    fixture.componentRef.setInput('isEmpty', isEmpty);
                    fixture.componentRef.setInput('errorTitle', errorTitle);
                    fixture.detectChanges();

                    const skeleton = fixture.nativeElement.querySelector('ui-skeleton-grid');
                    const error = fixture.nativeElement.querySelector('ui-error-state');
                    const empty = fixture.nativeElement.querySelector('ui-empty-state');
                    const grid = fixture.nativeElement.querySelector('.card-grid');

                    const activeStates = [skeleton, error, empty, grid].filter(
                        (el) => el !== null,
                    ).length;

                    expect(activeStates).toBe(1);
                },
            ),
        );
    });
});
