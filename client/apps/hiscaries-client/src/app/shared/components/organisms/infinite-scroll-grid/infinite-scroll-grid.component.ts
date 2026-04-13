import { IntersectionAnchorComponent } from '../../atoms/intersection-anchor/intersection-anchor.component';
import { CardGridComponent } from '../card-grid/card-grid.component';
import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  selector: 'ui-infinite-scroll-grid',
  standalone: true,
  imports: [CardGridComponent, IntersectionAnchorComponent],
  templateUrl: './infinite-scroll-grid.component.html',
  styleUrl: './infinite-scroll-grid.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InfiniteScrollGridComponent {
  readonly isLoading = input(false);
  readonly isEmpty = input(false);
  readonly hasMore = input(true);
  readonly errorTitle = input<string | null>(null);
  readonly errorDescription = input<string | null>(null);
  readonly emptyIcon = input('📚');
  readonly emptyMessage = input('Nothing here yet.');
  readonly skeletonCount = input(3);
  readonly minCardWidth = input('300px');

  readonly loadMore = output<void>();

  onIntersected(): void {
    if (this.hasMore() && !this.isLoading()) {
      this.loadMore.emit();
    }
  }
}
