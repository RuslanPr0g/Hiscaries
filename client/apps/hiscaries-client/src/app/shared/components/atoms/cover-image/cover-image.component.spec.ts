import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CoverImageComponent } from './cover-image.component';

describe('CoverImageComponent', () => {
    let fixture: ComponentFixture<CoverImageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CoverImageComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CoverImageComponent);
        fixture.detectChanges();
    });

    // Requirements 5.1 – <img> is rendered when src is non-null
    it('should render an <img> element when src is provided', () => {
        fixture.componentRef.setInput('src', 'https://example.com/cover.jpg');
        fixture.detectChanges();

        const img: HTMLImageElement | null = fixture.nativeElement.querySelector('img.cover-img');
        expect(img).not.toBeNull();
    });

    // Requirements 5.1 – placeholder rendered when src is null
    it('should render the placeholder div when src is null', () => {
        fixture.componentRef.setInput('src', null);
        fixture.detectChanges();

        const placeholder: HTMLElement | null = fixture.nativeElement.querySelector('.cover-placeholder');
        expect(placeholder).not.toBeNull();
    });

    // Requirements 5.1 – <img> not rendered when src is null
    it('should not render an <img> element when src is null', () => {
        fixture.componentRef.setInput('src', null);
        fixture.detectChanges();

        const img: HTMLImageElement | null = fixture.nativeElement.querySelector('img.cover-img');
        expect(img).toBeNull();
    });

    // Requirements 5.2 – gradient overlay present when showGradient=true
    it('should render the gradient overlay when showGradient is true', () => {
        fixture.componentRef.setInput('showGradient', true);
        fixture.detectChanges();

        const gradient: HTMLElement | null = fixture.nativeElement.querySelector('.cover-gradient');
        expect(gradient).not.toBeNull();
    });

    // Requirements 5.2 – gradient overlay absent when showGradient=false
    it('should not render the gradient overlay when showGradient is false', () => {
        fixture.componentRef.setInput('showGradient', false);
        fixture.detectChanges();

        const gradient: HTMLElement | null = fixture.nativeElement.querySelector('.cover-gradient');
        expect(gradient).toBeNull();
    });

    // Requirements 5.3 – loading="lazy" when lazy=true
    it('should set loading="lazy" on the img when lazy is true', () => {
        fixture.componentRef.setInput('src', 'https://example.com/cover.jpg');
        fixture.componentRef.setInput('lazy', true);
        fixture.detectChanges();

        const img: HTMLImageElement = fixture.nativeElement.querySelector('img.cover-img');
        expect(img.getAttribute('loading')).toBe('lazy');
    });

    // Requirements 5.3 – loading="eager" when lazy=false
    it('should set loading="eager" on the img when lazy is false', () => {
        fixture.componentRef.setInput('src', 'https://example.com/cover.jpg');
        fixture.componentRef.setInput('lazy', false);
        fixture.detectChanges();

        const img: HTMLImageElement = fixture.nativeElement.querySelector('img.cover-img');
        expect(img.getAttribute('loading')).toBe('eager');
    });

    // Requirements 5.4 – alt attribute is set on the <img>
    it('should set the alt attribute on the <img> element', () => {
        fixture.componentRef.setInput('src', 'https://example.com/cover.jpg');
        fixture.componentRef.setInput('alt', 'Story cover');
        fixture.detectChanges();

        const img: HTMLImageElement = fixture.nativeElement.querySelector('img.cover-img');
        expect(img.getAttribute('alt')).toBe('Story cover');
    });

    // Requirements 5.5 – aspectRatio applied as padding-bottom on the ratio container
    it('should apply the aspectRatio value as padding-bottom on the ratio container', () => {
        fixture.componentRef.setInput('aspectRatio', '56%');
        fixture.detectChanges();

        const ratioDiv: HTMLElement = fixture.nativeElement.querySelector('.cover-ratio');
        expect(ratioDiv.style.paddingBottom).toBe('56%');
    });
});
