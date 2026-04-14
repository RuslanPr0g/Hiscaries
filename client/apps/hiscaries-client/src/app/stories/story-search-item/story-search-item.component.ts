import { CommonModule } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { ProgressBarComponent } from '@shared/components/progress-bar/progress-bar.component';
import { NavigationConst } from '@shared/constants/navigation.const';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';
import { StoryModel } from '@stories/models/domain/story-model';

@Component({
  selector: 'app-story-search-item',
  standalone: true,
  imports: [CommonModule, ProgressBarComponent, FallbackImagePipe],
  templateUrl: './story-search-item.component.html',
  styleUrl: './story-search-item.component.scss',
})
export class SearchStoryItemComponent {
  private router = inject(Router);

  readonly story = input<StoryModel>();

  previewStory(story: StoryModel): void {
    this.router.navigate([NavigationConst.PreviewStory(story.Id)]);
  }
}
