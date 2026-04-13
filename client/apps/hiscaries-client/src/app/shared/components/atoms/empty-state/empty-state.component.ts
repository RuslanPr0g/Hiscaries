import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
    selector: 'ui-empty-state',
    standalone: true,
    templateUrl: './empty-state.component.html',
    styleUrl: './empty-state.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmptyStateComponent {
    readonly icon = input('📚');
    readonly message = input.required<string>();
    readonly description = input<string | null>(null);
}
