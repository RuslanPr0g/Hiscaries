import { Component, inject, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { SearchStoryResultsComponent } from '@stories/search-story-results/search-story-results.component';
import { UserStoryService } from '@user-to-story/services/multiple-services-merged/user-story.service';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { ReadHistoryStory } from '@stories/models/domain/story-model';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';

@Component({
  selector: 'app-reading-history',
  standalone: true,
  imports: [CommonModule, SearchStoryResultsComponent],
  templateUrl: './reading-history.component.html',
  styleUrls: ['./reading-history.component.scss'],
  providers: [PaginationService],
})
export class ReadingHistoryComponent implements AfterViewInit {
  private userStoryService = inject(UserStoryService);
  pagination = inject(PaginationService);

  stories = signal<QueriedModel<ReadHistoryStory>>(generateEmptyQueriedResult());
  groupedStories = signal<{ [key: string]: ReadHistoryStory[] }>({});
  isLoading = signal(false);

  @ViewChild('loadMoreAnchor', { static: true }) loadMoreAnchor!: ElementRef<HTMLDivElement>;
  private observer!: IntersectionObserver;

  constructor() {
    this.loadStories(true);
  }

  ngAfterViewInit() {
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting && !this.isLoading()) {
            this.nextPage();
          }
        });
      },
      { threshold: 0 },
    );

    if (this.loadMoreAnchor) {
      this.observer.observe(this.loadMoreAnchor.nativeElement);
    }
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

  private groupByDate(stories: ReadHistoryStory[]): { [key: string]: ReadHistoryStory[] } {
    const groups: { [key: string]: ReadHistoryStory[] } = {};
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
}
