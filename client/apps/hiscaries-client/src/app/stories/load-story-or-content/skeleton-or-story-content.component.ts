import { Component, input } from '@angular/core';

import { Skeleton } from 'primeng/skeleton';
import { TemplateMessageModel } from '@stories/models/template-message.model';

@Component({
  selector: 'app-skeleton-or-story-content',
  standalone: true,
  imports: [Skeleton],
  templateUrl: './skeleton-or-story-content.component.html',
  styleUrl: './skeleton-or-story-content.component.scss',
})
export class SkeletonOrStoryContentComponent {
  readonly dataLoaded = input(false);
  readonly errorMessage = input<TemplateMessageModel | null>();
}
