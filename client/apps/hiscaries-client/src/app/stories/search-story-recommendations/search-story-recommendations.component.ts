import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MediaCardComponent } from '@shared/components/molecules/media-card/media-card.component';
import { SectionHeaderComponent } from '@shared/components/molecules/section-header/section-header.component';
import { InfiniteScrollGridComponent } from '@shared/components/organisms/infinite-scroll-grid/infinite-scroll-grid.component';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { StoryModel } from '@stories/models/domain/story-model';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-search-story-recommendations',
  standalone: true,
  imports: [
    InfiniteScrollGridComponent,
    MediaCardComponent,
    SectionHeaderComponent,
    FallbackImagePipe,
  ],
  templateUrl: './search-story-recommendations.component.html',
  styleUrls: ['./search-story-recommendations.component.scss'],
  providers: [PaginationService],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchStoryRecommendationsComponent {
  private storyService = inject(StoryWithMetadataService);
  pagination = inject(PaginationService);

  stories = signal<QueriedModel<StoryModel>>(generateEmptyQueriedResult());
  isLoading = signal(false);

  constructor() {
    this.loadStories(true);
  }

  private loadStories(reset = false) {
    if (this.pagination.snapshot.StartIndex > 300) {
      return;
    }

    if (reset) {
      this.pagination.reset();
      this.stories.set(generateEmptyQueriedResult());
    }

    this.isLoading.set(true);

    this.storyService
      .recommendations(this.pagination.snapshot)
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
        }),
      )
      .subscribe((data) => {
        if (!data?.Items || data.Items.length === 0) {
          return;
        }

        const current = reset ? generateEmptyQueriedResult<StoryModel>() : this.stories();
        this.stories.set({
          Items: [...current.Items, ...data.Items],
          TotalItemsCount: data.TotalItemsCount,
        });
      });
  }

  nextPage() {
    this.pagination.nextPage();
    this.loadStories();
  }
}
