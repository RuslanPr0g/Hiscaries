import { Component, input } from '@angular/core';
import { TemplateMessageModel } from '@stories/models/template-message.model';
import { Skeleton } from 'primeng/skeleton';

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
