import {
    ChangeDetectionStrategy,
    Component,
    input,
    output,
} from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'ui-glass-card',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './glass-card.component.html',
    styleUrl: './glass-card.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GlassCardComponent {
    readonly interactive = input(false);
    readonly borderRadius = input<'sm' | 'md' | 'lg'>('lg');

    readonly cardClick = output<void>();

    onEnter(): void {
        if (this.interactive()) {
            this.cardClick.emit();
        }
    }
}
