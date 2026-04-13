import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BadgeComponent } from './badge.component';

describe('BadgeComponent', () => {
    let fixture: ComponentFixture<BadgeComponent>;

    function getBadge(): HTMLElement {
        return fixture.nativeElement.querySelector('.badge');
    }

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [BadgeComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(BadgeComponent);
        fixture.componentRef.setInput('label', 'Test');
        fixture.detectChanges();
    });

    // Requirements 3.1 – label text is rendered inside the badge element
    it('should render the label text inside the badge', () => {
        fixture.componentRef.setInput('label', 'Hello');
        fixture.detectChanges();

        expect(getBadge().textContent?.trim()).toBe('Hello');
    });

    // Requirements 3.2 – variant 'default' produces badge-default class
    it('should apply badge-default class when variant is "default"', () => {
        fixture.componentRef.setInput('variant', 'default');
        fixture.detectChanges();

        expect(getBadge().classList).toContain('badge-default');
        expect(getBadge().classList).not.toContain('badge-accent');
        expect(getBadge().classList).not.toContain('badge-danger');
        expect(getBadge().classList).not.toContain('badge-success');
    });

    // Requirements 3.2 – variant 'accent' produces badge-accent class
    it('should apply badge-accent class when variant is "accent"', () => {
        fixture.componentRef.setInput('variant', 'accent');
        fixture.detectChanges();

        expect(getBadge().classList).toContain('badge-accent');
        expect(getBadge().classList).not.toContain('badge-default');
        expect(getBadge().classList).not.toContain('badge-danger');
        expect(getBadge().classList).not.toContain('badge-success');
    });

    // Requirements 3.2 – variant 'danger' produces badge-danger class
    it('should apply badge-danger class when variant is "danger"', () => {
        fixture.componentRef.setInput('variant', 'danger');
        fixture.detectChanges();

        expect(getBadge().classList).toContain('badge-danger');
        expect(getBadge().classList).not.toContain('badge-default');
        expect(getBadge().classList).not.toContain('badge-accent');
        expect(getBadge().classList).not.toContain('badge-success');
    });

    // Requirements 3.2 – variant 'success' produces badge-success class
    it('should apply badge-success class when variant is "success"', () => {
        fixture.componentRef.setInput('variant', 'success');
        fixture.detectChanges();

        expect(getBadge().classList).toContain('badge-success');
        expect(getBadge().classList).not.toContain('badge-default');
        expect(getBadge().classList).not.toContain('badge-accent');
        expect(getBadge().classList).not.toContain('badge-danger');
    });

    // Requirements 3.3 – default variant is 'default'
    it('should use "default" variant when no variant is provided', () => {
        // No variant set — relies on default input value
        expect(getBadge().classList).toContain('badge-default');
    });
});
