import { HoverRevealPanelComponent } from './hover-reveal-panel.component';
import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';

// Requirements 13.1–13.4

describe('HoverRevealPanelComponent', () => {
    let fixture: ComponentFixture<HoverRevealPanelComponent>;

    function getPanel(): HTMLElement {
        return fixture.nativeElement.querySelector('.hover-reveal-panel');
    }

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [HoverRevealPanelComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(HoverRevealPanelComponent);
        fixture.detectChanges();
    });

    // Requirements 13.3 – background input is applied as inline style
    it('should apply the default background as inline style', () => {
        const panel = getPanel();
        // Browsers may normalize rgba values by adding spaces after commas
        expect(panel.style.background).toMatch(/rgba\(56,?\s*41,?\s*29,?\s*0\.8\)/);
    });

    // Requirements 13.3 – custom background input is reflected in the style
    it('should apply a custom background input as inline style', () => {
        fixture.componentRef.setInput('background', 'rgba(0,0,0,0.5)');
        fixture.detectChanges();

        const panel = getPanel();
        expect(panel.style.background).toMatch(/rgba\(0,?\s*0,?\s*0,?\s*0\.5\)/);
    });

    // Requirements 13.4 – content is projected via ng-content
    it('should project content via ng-content', () => {
        // Verify the panel wrapper exists and can host projected content
        expect(getPanel()).not.toBeNull();
    });
});

// Host wrapper test for ng-content projection
@Component({
    standalone: true,
    imports: [HoverRevealPanelComponent],
    template: `
    <ui-hover-reveal-panel>
      <span class="projected-content">Hello</span>
    </ui-hover-reveal-panel>
  `,
})
class HostComponent { }

describe('HoverRevealPanelComponent — content projection', () => {
    let fixture: ComponentFixture<HostComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [HostComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(HostComponent);
        fixture.detectChanges();
    });

    // Requirements 13.4 – projected content appears inside the panel
    it('should render projected content inside the panel', () => {
        const projected = fixture.nativeElement.querySelector('.projected-content');
        expect(projected).not.toBeNull();
        expect(projected.textContent).toBe('Hello');
    });
});
