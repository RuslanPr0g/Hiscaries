import { Component, inject, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { SearchStoryResultsComponent } from '@stories/search-story-results/search-story-results.component';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { StoryModel } from '@stories/models/domain/story-model';
import { emptyQueriedResult, QueriedModel } from '@shared/models/queried.model';

@Component({
  selector: 'app-search-story-recommendations',
  standalone: true,
  imports: [CommonModule, SearchStoryResultsComponent],
  templateUrl: './search-story-recommendations.component.html',
  styleUrls: ['./search-story-recommendations.component.scss'],
  providers: [PaginationService],
})
export class SearchStoryRecommendationsComponent implements AfterViewInit {
  private storyService = inject(StoryWithMetadataService);
  pagination = inject(PaginationService);

  stories = signal<QueriedModel<StoryModel>>(emptyQueriedResult);
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
      {
        threshold: 0,
      },
    );

    if (this.loadMoreAnchor) {
      this.observer.observe(this.loadMoreAnchor.nativeElement);
    }
  }

  private loadStories(reset: boolean = false) {
    if (this.pagination.snapshot.StartIndex > 300) {
      return;
    }

    if (reset) {
      this.pagination.reset();
      this.stories.set(emptyQueriedResult);
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
        const current = reset ? emptyQueriedResult : this.stories();
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
