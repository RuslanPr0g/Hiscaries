import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { MediaCardComponent } from '@shared/components/molecules/media-card/media-card.component';
import { CardGridComponent } from '@shared/components/organisms/card-grid/card-grid.component';
import { NavigationConst } from '@shared/constants/navigation.const';
import { QueriedModel } from '@shared/models/queried.model';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';
import { StoryModel } from '@stories/models/domain/story-model';

@Component({
  selector: 'app-search-story-results',
  standalone: true,
  imports: [CardGridComponent, MediaCardComponent, FallbackImagePipe],
  templateUrl: './search-story-results.component.html',
  styleUrls: ['./search-story-results.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchStoryResultsComponent {
  private router = inject(Router);

  readonly stories = input<QueriedModel<StoryModel> | null | undefined>(null);
  readonly isLoading = input<boolean | null>(false);
  readonly isCarousel = input(false);

  previewStory(story: StoryModel): void {
    this.router.navigate([NavigationConst.PreviewStory(story.Id)]);
  }
}
