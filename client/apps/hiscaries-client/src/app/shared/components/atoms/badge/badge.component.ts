import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
    selector: 'ui-badge',
    standalone: true,
    templateUrl: './badge.component.html',
    styleUrl: './badge.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BadgeComponent {
    readonly label = input.required<string>();
    readonly variant = input<'default' | 'accent' | 'danger' | 'success'>('default');
}
