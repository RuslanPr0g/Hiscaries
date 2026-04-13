import {
    ChangeDetectionStrategy,
    Component,
    input,
    output,
} from '@angular/core';
import { CoverImageComponent } from '../../atoms/cover-image/cover-image.component';
import { TruncatedTextComponent } from '../../atoms/truncated-text/truncated-text.component';
import { HoverRevealPanelComponent } from '../hover-reveal-panel/hover-reveal-panel.component';
import { ProgressBarComponent } from '../../progress-bar/progress-bar.component';

@Component({
    selector: 'ui-media-card',
    standalone: true,
    imports: [
        CoverImageComponent,
        TruncatedTextComponent,
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
    readonly progressPercentage = input<number | null>(null);
    readonly interactive = input(true);

    readonly clicked = output<void>();

    onInteract(): void {
        this.clicked.emit();
    }
}
