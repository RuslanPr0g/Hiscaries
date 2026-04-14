import { SkeletonCardComponent } from '../../atoms/skeleton-card/skeleton-card.component';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'ui-skeleton-grid',
  standalone: true,
  imports: [SkeletonCardComponent],
  templateUrl: './skeleton-grid.component.html',
  styleUrl: './skeleton-grid.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkeletonGridComponent {
  readonly count = input(3);
  readonly imageHeight = input('170px');
  readonly skeletonLines = input(2);

  protected readonly items = computed(() => Array.from({ length: this.count() }));
}
