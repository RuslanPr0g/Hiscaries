import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AvatarImageComponent } from './avatar-image.component';

describe('AvatarImageComponent', () => {
    let fixture: ComponentFixture<AvatarImageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AvatarImageComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AvatarImageComponent);
        fixture.detectChanges();
    });

    // Requirements 4.1 – <img> is rendered when src is a non-null string
    it('should render an <img> element when src is a non-null string', () => {
        fixture.componentRef.setInput('src', 'https://example.com/avatar.png');
        fixture.detectChanges();

        const img: HTMLImageElement | null = fixture.nativeElement.querySelector('img');
        expect(img).not.toBeNull();
    });

    // Requirements 4.1 – initials span is not rendered when src is provided
    it('should not render the initials span when src is a non-null string', () => {
        fixture.componentRef.setInput('src', 'https://example.com/avatar.png');
        fixture.detectChanges();

        const span: HTMLElement | null = fixture.nativeElement.querySelector('span.avatar-initials');
        expect(span).toBeNull();
    });

    // Requirements 4.2 – initials span is rendered when src is null
    it('should render the initials span when src is null', () => {
        fixture.componentRef.setInput('src', null);
        fixture.componentRef.setInput('initials', 'AB');
        fixture.detectChanges();

        const span: HTMLElement | null = fixture.nativeElement.querySelector('span');
        expect(span).not.toBeNull();
        expect(span?.textContent?.trim()).toBe('AB');
    });

    // Requirements 4.2 – <img> is not rendered when src is null
    it('should not render an <img> element when src is null', () => {
        fixture.componentRef.setInput('src', null);
        fixture.detectChanges();

        const img: HTMLImageElement | null = fixture.nativeElement.querySelector('img');
        expect(img).toBeNull();
    });

    // Requirements 4.3 – size 'sm' maps to avatar-sm class (40px)
    it('should apply avatar-sm class when size is "sm"', () => {
        fixture.componentRef.setInput('src', 'https://example.com/avatar.png');
        fixture.componentRef.setInput('size', 'sm');
        fixture.detectChanges();

        const img: HTMLImageElement = fixture.nativeElement.querySelector('img');
        expect(img.classList).toContain('avatar-sm');
        expect(img.classList).not.toContain('avatar-md');
        expect(img.classList).not.toContain('avatar-lg');
    });

    // Requirements 4.3 – size 'md' maps to avatar-md class (80px)
    it('should apply avatar-md class when size is "md"', () => {
        fixture.componentRef.setInput('src', 'https://example.com/avatar.png');
        fixture.componentRef.setInput('size', 'md');
        fixture.detectChanges();

        const img: HTMLImageElement = fixture.nativeElement.querySelector('img');
        expect(img.classList).toContain('avatar-md');
        expect(img.classList).not.toContain('avatar-sm');
        expect(img.classList).not.toContain('avatar-lg');
    });

    // Requirements 4.3 – size 'lg' maps to avatar-lg class (120px)
    it('should apply avatar-lg class when size is "lg"', () => {
        fixture.componentRef.setInput('src', 'https://example.com/avatar.png');
        fixture.componentRef.setInput('size', 'lg');
        fixture.detectChanges();

        const img: HTMLImageElement = fixture.nativeElement.querySelector('img');
        expect(img.classList).toContain('avatar-lg');
        expect(img.classList).not.toContain('avatar-sm');
        expect(img.classList).not.toContain('avatar-md');
    });

    // Requirements 4.3 – size classes also apply to the initials span
    it('should apply size class to the initials span when src is null', () => {
        fixture.componentRef.setInput('src', null);
        fixture.componentRef.setInput('size', 'lg');
        fixture.detectChanges();

        const span: HTMLElement = fixture.nativeElement.querySelector('span');
        expect(span.classList).toContain('avatar-lg');
    });

    // Requirements 4.4 – alt attribute is set on the <img> element
    it('should set the alt attribute on the <img> element', () => {
        fixture.componentRef.setInput('src', 'https://example.com/avatar.png');
        fixture.componentRef.setInput('alt', 'User profile picture');
        fixture.detectChanges();

        const img: HTMLImageElement = fixture.nativeElement.querySelector('img');
        expect(img.getAttribute('alt')).toBe('User profile picture');
    });

    // Requirements 4.4 – alt defaults to "Avatar" when not provided
    it('should use "Avatar" as the default alt text', () => {
        fixture.componentRef.setInput('src', 'https://example.com/avatar.png');
        fixture.detectChanges();

        const img: HTMLImageElement = fixture.nativeElement.querySelector('img');
        expect(img.getAttribute('alt')).toBe('Avatar');
    });
});
