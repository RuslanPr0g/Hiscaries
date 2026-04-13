import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MediaCardComponent } from '@shared/components/molecules/media-card/media-card.component';
import { SectionHeaderComponent } from '@shared/components/molecules/section-header/section-header.component';
import { InfiniteScrollGridComponent } from '@shared/components/organisms/infinite-scroll-grid/infinite-scroll-grid.component';
import { NavigationConst } from '@shared/constants/navigation.const';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { StoryModel } from '@stories/models/domain/story-model';
import { UserStoryService } from '@user-to-story/services/multiple-services-merged/user-story.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-search-story-resume-reading',
  standalone: true,
  imports: [
    InfiniteScrollGridComponent,
    MediaCardComponent,
    SectionHeaderComponent,
    FallbackImagePipe,
  ],
  templateUrl: './search-story-resume-reading.component.html',
  styleUrls: ['./search-story-resume-reading.component.scss'],
  providers: [PaginationService],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchStoryResumeReadingComponent {
  private userStoryService = inject(UserStoryService);
  private router = inject(Router);
  pagination = inject(PaginationService);

  stories = signal<QueriedModel<StoryModel>>(generateEmptyQueriedResult());
  isLoading = signal(false);

  constructor() {
    this.loadStories(true);
  }

  private loadStories(reset = false) {
    if (reset) {
      this.pagination.reset();
      this.stories.set(generateEmptyQueriedResult());
    }

    this.isLoading.set(true);
    this.userStoryService
      .resumeReading(this.pagination.snapshot)
      .pipe(finalize(() => this.isLoading.set(false)))
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

  prevPage() {
    if (this.pagination.snapshot.StartIndex === 0) return;
    this.pagination.prevPage();
    this.loadStories();
  }

  previewStory(story: StoryModel): void {
    this.router.navigate([NavigationConst.PreviewStory(story.Id)]);
  }
}
