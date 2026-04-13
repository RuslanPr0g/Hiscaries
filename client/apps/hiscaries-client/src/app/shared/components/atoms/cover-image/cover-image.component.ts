import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
    selector: 'ui-cover-image',
    standalone: true,
    templateUrl: './cover-image.component.html',
    styleUrl: './cover-image.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CoverImageComponent {
    readonly src = input<string | null>(null);
    readonly alt = input('');
    readonly aspectRatio = input('43%');
    readonly showGradient = input(true);
    readonly lazy = input(true);
}
