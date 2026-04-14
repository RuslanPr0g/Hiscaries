import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { IntersectionAnchorComponent } from '@shared/components/atoms/intersection-anchor/intersection-anchor.component';
import { MediaCardComponent } from '@shared/components/molecules/media-card/media-card.component';
import { SectionHeaderComponent } from '@shared/components/molecules/section-header/section-header.component';
import { CardGridComponent } from '@shared/components/organisms/card-grid/card-grid.component';
import { NavigationConst } from '@shared/constants/navigation.const';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { ReadHistoryStory } from '@stories/models/domain/story-model';
import { UserStoryService } from '@user-to-story/services/multiple-services-merged/user-story.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-reading-history',
  standalone: true,
  imports: [
    CardGridComponent,
    IntersectionAnchorComponent,
    MediaCardComponent,
    SectionHeaderComponent,
    FallbackImagePipe,
  ],
  templateUrl: './reading-history.component.html',
  styleUrls: ['./reading-history.component.scss'],
  providers: [PaginationService],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReadingHistoryComponent {
  private userStoryService = inject(UserStoryService);
  private router = inject(Router);
  pagination = inject(PaginationService);

  stories = signal<QueriedModel<ReadHistoryStory>>(generateEmptyQueriedResult());
  groupedStories = signal<Record<string, ReadHistoryStory[]>>({});
  isLoading = signal(false);

  constructor() {
    this.loadStories(true);
  }

  public get storyKeys() {
    return Object.keys(this.groupedStories());
  }

  private loadStories(reset = false) {
    if (reset) {
      this.pagination.reset();
      this.stories.set(generateEmptyQueriedResult());
      this.groupedStories.set({});
    }

    this.isLoading.set(true);
    this.userStoryService
      .readingHistory(this.pagination.snapshot)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe((data) => {
        const current = reset ? generateEmptyQueriedResult<ReadHistoryStory>() : this.stories();
        const combined = {
          Items: [...current.Items, ...data.Items],
          TotalItemsCount: data.TotalItemsCount,
        };
        this.stories.set(combined);
        this.groupedStories.set(this.groupByDate(combined.Items));
      });
  }

  private groupByDate(stories: ReadHistoryStory[]): Record<string, ReadHistoryStory[]> {
    const groups: Record<string, ReadHistoryStory[]> = {};
    const now = new Date();

    const dayLabels = (date: Date) => {
      const diffMs = now.getTime() - date.getTime();
      const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
      if (diffDays === 0) return 'Today';
      if (diffDays === 1) return 'Yesterday';
      if (diffDays < 7) return date.toLocaleDateString(undefined, { weekday: 'long' });
      if (diffDays < 30) return 'A week ago';
      if (diffDays < 365) return 'A month ago';
      if (diffDays < 665) return 'A year ago';
      return 'Years ago';
    };

    stories.forEach((story) => {
      const date = new Date(story.LastReadAt || story.DatePublished || new Date());
      const label = dayLabels(date);
      if (!groups[label]) groups[label] = [];
      groups[label].push(story);
    });

    return groups;
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

  previewStory(story: ReadHistoryStory): void {
    this.router.navigate([NavigationConst.PreviewStory(story.Id)]);
  }
}
