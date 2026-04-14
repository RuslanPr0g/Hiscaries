import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'ui-hover-reveal-panel',
  standalone: true,
  imports: [],
  templateUrl: './hover-reveal-panel.component.html',
  styleUrl: './hover-reveal-panel.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class.is-visible]': 'visible()',
  },
})
export class HoverRevealPanelComponent {
  readonly background = input('rgba(30,18,10,0.88)');
  readonly visible = input(false);
}
