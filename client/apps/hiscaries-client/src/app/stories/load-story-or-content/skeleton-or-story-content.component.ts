import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkeletonModule } from 'primeng/skeleton';
import { TemplateMessageModel } from '@stories/models/template-message.model';

@Component({
  selector: 'app-skeleton-or-story-content',
  standalone: true,
  imports: [CommonModule, SkeletonModule],
  templateUrl: './skeleton-or-story-content.component.html',
  styleUrl: './skeleton-or-story-content.component.scss',
})
export class SkeletonOrStoryContentComponent {
  @Input() dataLoaded = false;
  @Input() errorMessage?: TemplateMessageModel | null;
}
