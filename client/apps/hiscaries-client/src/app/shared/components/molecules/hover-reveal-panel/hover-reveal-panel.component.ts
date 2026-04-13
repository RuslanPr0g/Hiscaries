import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
    selector: 'ui-hover-reveal-panel',
    standalone: true,
    imports: [],
    templateUrl: './hover-reveal-panel.component.html',
    styleUrl: './hover-reveal-panel.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HoverRevealPanelComponent {
    readonly background = input('rgba(56,41,29,0.8)');
}
