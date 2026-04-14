import { GlassCardComponent } from '../glass-card/glass-card.component';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'ui-skeleton-card',
  standalone: true,
  imports: [SkeletonModule, GlassCardComponent],
  templateUrl: './skeleton-card.component.html',
  styleUrl: './skeleton-card.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkeletonCardComponent {
  readonly imageHeight = input('170px');
  readonly lines = input(2);

  protected readonly lineItems = computed(() => Array.from({ length: this.lines() }));
}
