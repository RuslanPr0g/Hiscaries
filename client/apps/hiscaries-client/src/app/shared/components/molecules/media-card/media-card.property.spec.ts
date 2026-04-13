import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';
import { MediaCardComponent } from './media-card.component';

/**
 * Property 14: MediaCard progress bar visibility matches input nullability
 *
 * For any `progressPercentage` value, the `MediaCard` SHALL render the
 * `ProgressBarComponent` if and only if `progressPercentage` is a non-null number.
 *
 * Validates: Requirements 11.2, 11.3
 */
describe('MediaCardComponent — Property 14: progress bar visibility matches input nullability', () => {
    let fixture: ComponentFixture<MediaCardComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MediaCardComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(MediaCardComponent);
        fixture.componentRef.setInput('title', 'Test Title');
        fixture.detectChanges();
    });

    it('should render progress bar if and only if progressPercentage is a non-null number', () => {
        fc.assert(
            fc.property(
                fc.option(fc.float({ min: 0, max: 100 }), { nil: null }),
                (progressPercentage) => {
                    fixture.componentRef.setInput('progressPercentage', progressPercentage);
                    fixture.detectChanges();

                    const progressBar = fixture.nativeElement.querySelector('app-progress-bar');
                    const isRendered = progressBar !== null;
                    const isNonNull = progressPercentage !== null;

                    expect(isRendered).toBe(isNonNull);
                },
            ),
        );
    });
});
