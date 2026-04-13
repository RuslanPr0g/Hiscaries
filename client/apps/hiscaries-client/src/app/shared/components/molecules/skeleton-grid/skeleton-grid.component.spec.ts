import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SkeletonGridComponent } from './skeleton-grid.component';

// Requirements 12.1–12.3

describe('SkeletonGridComponent', () => {
    let fixture: ComponentFixture<SkeletonGridComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [SkeletonGridComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(SkeletonGridComponent);
        fixture.detectChanges();
    });

    // Requirements 12.1 – default count renders 3 skeleton cards
    it('should render 3 skeleton cards by default', () => {
        const cards = fixture.nativeElement.querySelectorAll('ui-skeleton-card');
        expect(cards.length).toBe(3);
    });

    // Requirements 12.1 – count=1 renders exactly 1 skeleton card
    it('should render exactly 1 skeleton card when count is 1', () => {
        fixture.componentRef.setInput('count', 1);
        fixture.detectChanges();

        const cards = fixture.nativeElement.querySelectorAll('ui-skeleton-card');
        expect(cards.length).toBe(1);
    });

    // Requirements 12.1 – count=5 renders exactly 5 skeleton cards
    it('should render exactly 5 skeleton cards when count is 5', () => {
        fixture.componentRef.setInput('count', 5);
        fixture.detectChanges();

        const cards = fixture.nativeElement.querySelectorAll('ui-skeleton-card');
        expect(cards.length).toBe(5);
    });

    // Requirements 12.1 – count=0 renders no skeleton cards
    it('should render no skeleton cards when count is 0', () => {
        fixture.componentRef.setInput('count', 0);
        fixture.detectChanges();

        const cards = fixture.nativeElement.querySelectorAll('ui-skeleton-card');
        expect(cards.length).toBe(0);
    });

    // Requirements 12.2 – imageHeight is passed through to each SkeletonCard
    it('should pass imageHeight to each skeleton card', () => {
        fixture.componentRef.setInput('count', 2);
        fixture.componentRef.setInput('imageHeight', '250px');
        fixture.detectChanges();

        const cards = fixture.nativeElement.querySelectorAll('ui-skeleton-card');
        expect(cards.length).toBe(2);
        // Each card's first p-skeleton (image) should have the custom height applied
        cards.forEach((card: Element) => {
            const imageSkeleton = card.querySelector('p-skeleton') as HTMLElement;
            expect(imageSkeleton).not.toBeNull();
            expect(imageSkeleton.style.height).toBe('250px');
        });
    });

    // Requirements 12.3 – skeletonLines is passed through to each SkeletonCard as lines
    it('should pass skeletonLines to each skeleton card as lines', () => {
        fixture.componentRef.setInput('count', 2);
        fixture.componentRef.setInput('skeletonLines', 4);
        fixture.detectChanges();

        const cards = fixture.nativeElement.querySelectorAll('ui-skeleton-card');
        expect(cards.length).toBe(2);
        // Each card with 4 lines should render 4 skeleton line elements (plus 1 image skeleton)
        cards.forEach((card: Element) => {
            // 1 image skeleton + 4 line skeletons = 5 total p-skeleton elements per card
            const skeletons = card.querySelectorAll('p-skeleton');
            expect(skeletons.length).toBe(5);
        });
    });
});
