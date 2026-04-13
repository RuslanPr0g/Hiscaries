import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-glass-overlay',
  standalone: true,
  templateUrl: './glass-overlay.component.html',
  styleUrl: './glass-overlay.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GlassOverlayComponent {
  readonly opacity = input(0.7);
  readonly color = input('0,0,0');
  readonly visible = input(true);
  readonly blur = input(false);

  protected readonly bgStyle = computed(() => `rgba(${this.color()},${this.opacity()})`);
}
