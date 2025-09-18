import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { StoryModel } from '@stories/models/domain/story-model';
import { Router } from '@angular/router';
import { NavigationConst } from '@shared/constants/navigation.const';
import { ProgressBarComponent } from '@shared/components/progress-bar/progress-bar.component';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';

@Component({
  selector: 'app-story-search-item',
  standalone: true,
  imports: [CommonModule, ProgressBarComponent, FallbackImagePipe],
  templateUrl: './story-search-item.component.html',
  styleUrl: './story-search-item.component.scss',
})
export class SearchStoryItemComponent {
  @Input() story: StoryModel;

  constructor(private router: Router) {}

  previewStory(story: StoryModel): void {
    this.router.navigate([NavigationConst.PreviewStory(story.Id)]);
  }
}
