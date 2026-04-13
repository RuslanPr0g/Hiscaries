import {
    ChangeDetectionStrategy,
    Component,
    computed,
    input,
} from '@angular/core';
import { EmptyStateComponent } from '../../atoms/empty-state/empty-state.component';
import { ErrorStateComponent } from '../../atoms/error-state/error-state.component';
import { SkeletonGridComponent } from '../../molecules/skeleton-grid/skeleton-grid.component';

@Component({
    selector: 'ui-card-grid',
    standalone: true,
    imports: [SkeletonGridComponent, ErrorStateComponent, EmptyStateComponent],
    templateUrl: './card-grid.component.html',
    styleUrl: './card-grid.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CardGridComponent {
    readonly isLoading = input(false);
    readonly isEmpty = input(false);
    readonly errorTitle = input<string | null>(null);
    readonly errorDescription = input<string | null>(null);
    readonly emptyIcon = input('📚');
    readonly emptyMessage = input('Nothing here yet.');
    readonly skeletonCount = input(3);
    readonly minCardWidth = input('300px');

    protected readonly renderState = computed<'skeleton' | 'error' | 'empty' | 'content'>(() => {
        if (this.isLoading()) return 'skeleton';
        if (this.errorTitle() !== null) return 'error';
        if (this.isEmpty()) return 'empty';
        return 'content';
    });

    protected readonly gridCols = computed(
        () => `repeat(auto-fill, minmax(${this.minCardWidth()}, 1fr))`,
    );
}
