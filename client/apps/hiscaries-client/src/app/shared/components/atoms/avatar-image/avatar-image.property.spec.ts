import { ComponentFixture, TestBed } from '@angular/core/testing';
import * as fc from 'fast-check';
import { AvatarImageComponent } from './avatar-image.component';

/**
 * Property 2: AvatarImage size always produces a dimension class
 *
 * For any `size` value in the union `('sm' | 'md' | 'lg')`, the `AvatarImage`
 * component SHALL apply a CSS class that maps to the correct pixel dimension
 * (40px / 80px / 120px).
 *
 * Validates: Requirements 4.3
 */
describe('AvatarImageComponent — Property 2: size always produces a dimension class', () => {
    let fixture: ComponentFixture<AvatarImageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AvatarImageComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AvatarImageComponent);
        fixture.detectChanges();
    });

    const sizeArb = fc.constantFrom('sm', 'md', 'lg') as fc.Arbitrary<'sm' | 'md' | 'lg'>;
    const allSizes: Array<'sm' | 'md' | 'lg'> = ['sm', 'md', 'lg'];

    it('should apply the correct avatar-{size} class when src is provided', () => {
        fc.assert(
            fc.property(sizeArb, (size) => {
                fixture.componentRef.setInput('src', 'https://example.com/avatar.png');
                fixture.componentRef.setInput('size', size);
                fixture.detectChanges();

                const el: HTMLElement = fixture.nativeElement.querySelector('.avatar');
                const expectedClass = `avatar-${size}`;

                expect(el).not.toBeNull();
                expect(el.classList).toContain(expectedClass);

                // Only the current size class should be applied
                const otherSizes = allSizes.filter((s) => s !== size);
                for (const other of otherSizes) {
                    expect(el.classList).not.toContain(`avatar-${other}`);
                }
            }),
        );
    });

    it('should apply the correct avatar-{size} class when src is null (initials fallback)', () => {
        fc.assert(
            fc.property(sizeArb, (size) => {
                fixture.componentRef.setInput('src', null);
                fixture.componentRef.setInput('initials', 'AB');
                fixture.componentRef.setInput('size', size);
                fixture.detectChanges();

                const el: HTMLElement = fixture.nativeElement.querySelector('.avatar');
                const expectedClass = `avatar-${size}`;

                expect(el).not.toBeNull();
                expect(el.classList).toContain(expectedClass);

                // Only the current size class should be applied
                const otherSizes = allSizes.filter((s) => s !== size);
                for (const other of otherSizes) {
                    expect(el.classList).not.toContain(`avatar-${other}`);
                }
            }),
        );
    });
});
