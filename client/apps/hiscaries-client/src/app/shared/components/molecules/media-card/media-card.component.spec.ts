import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MediaCardComponent } from './media-card.component';

// Requirements 11.1–11.9, 19.1, 19.2

describe('MediaCardComponent', () => {
    let fixture: ComponentFixture<MediaCardComponent>;
    let component: MediaCardComponent;

    function getCard(): HTMLElement {
        return fixture.nativeElement.querySelector('.media-card');
    }

    function getProgressBar(): HTMLElement | null {
        return fixture.nativeElement.querySelector('app-progress-bar');
    }

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MediaCardComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(MediaCardComponent);
        component = fixture.componentInstance;
        fixture.componentRef.setInput('title', 'Test Title');
        fixture.detectChanges();
    });

    // Requirements 19.1 – tabindex="0" is present for keyboard accessibility
    it('should have tabindex="0" on the card element', () => {
        expect(getCard().getAttribute('tabindex')).toBe('0');
    });

    // Requirements 11.4 – clicked emits on click
    it('should emit clicked when the card is clicked', () => {
        let emitted = false;
        component.clicked.subscribe(() => (emitted = true));

        getCard().click();
        expect(emitted).toBe(true);
    });

    // Requirements 11.5, 19.2 – clicked emits on Enter keydown
    it('should emit clicked when Enter is pressed', () => {
        let emitted = false;
        component.clicked.subscribe(() => (emitted = true));

        const event = new KeyboardEvent('keydown', { key: 'Enter', bubbles: true });
        getCard().dispatchEvent(event);
        expect(emitted).toBe(true);
    });

    // Requirements 11.2 – progress bar rendered when progressPercentage is non-null
    it('should render the progress bar when progressPercentage is a non-null number', () => {
        fixture.componentRef.setInput('progressPercentage', 50);
        fixture.detectChanges();

        expect(getProgressBar()).not.toBeNull();
    });

    // Requirements 11.3 – progress bar absent when progressPercentage is null
    it('should not render the progress bar when progressPercentage is null', () => {
        fixture.componentRef.setInput('progressPercentage', null);
        fixture.detectChanges();

        expect(getProgressBar()).toBeNull();
    });

    // Requirements 11.3 – progress bar absent by default (default is null)
    it('should not render the progress bar by default', () => {
        expect(getProgressBar()).toBeNull();
    });
});

// Host wrapper test for ng-content projection
@Component({
    standalone: true,
    imports: [MediaCardComponent],
    template: `
    <ui-media-card title="Test">
      <div card-footer class="footer-content">Footer</div>
    </ui-media-card>
  `,
})
class HostComponent { }

describe('MediaCardComponent — card-footer slot projection', () => {
    let fixture: ComponentFixture<HostComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [HostComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(HostComponent);
        fixture.detectChanges();
    });

    // Requirements 11.7 – card-footer slot content is projected
    it('should project content into the card-footer slot', () => {
        const footer = fixture.nativeElement.querySelector('.footer-content');
        expect(footer).not.toBeNull();
        expect(footer.textContent).toBe('Footer');
    });
});
