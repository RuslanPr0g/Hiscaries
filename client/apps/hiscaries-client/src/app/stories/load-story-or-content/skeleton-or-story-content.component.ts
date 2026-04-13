import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ErrorStateComponent } from '@shared/components/atoms/error-state/error-state.component';
import { SkeletonGridComponent } from '@shared/components/molecules/skeleton-grid/skeleton-grid.component';
import { TemplateMessageModel } from '@stories/models/template-message.model';

@Component({
  selector: 'app-skeleton-or-story-content',
  standalone: true,
  imports: [SkeletonGridComponent, ErrorStateComponent],
  templateUrl: './skeleton-or-story-content.component.html',
  styleUrl: './skeleton-or-story-content.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkeletonOrStoryContentComponent {
  readonly dataLoaded = input(false);
  readonly errorMessage = input<TemplateMessageModel | null>();
}
