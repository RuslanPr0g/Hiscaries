import { CoverImageComponent } from '../../atoms/cover-image/cover-image.component';
import { GlassCardComponent } from '../../atoms/glass-card/glass-card.component';
import { ProgressBarComponent } from '../../progress-bar/progress-bar.component';
import { HoverRevealPanelComponent } from '../hover-reveal-panel/hover-reveal-panel.component';
import { ChangeDetectionStrategy, Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'ui-media-card',
  standalone: true,
  imports: [
    GlassCardComponent,
    CoverImageComponent,
    HoverRevealPanelComponent,
    ProgressBarComponent,
  ],
  templateUrl: './media-card.component.html',
  styleUrl: './media-card.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MediaCardComponent {
  readonly imageSrc = input<string | null>(null);
  readonly imageAlt = input('');
  readonly title = input.required<string>();
  readonly subtitle = input<string | null>(null);
  readonly description = input<string | null>(null);
  readonly ageLimit = input<number | null>(null);
  readonly progressPercentage = input<number | null>(null);
  readonly interactive = input(true);

  readonly clicked = output<void>();

  protected readonly hovered = signal(false);

  onInteract(): void {
    this.clicked.emit();
  }
}
