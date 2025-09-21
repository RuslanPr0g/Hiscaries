import { Component, inject, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { SearchStoryResultsComponent } from '@stories/search-story-results/search-story-results.component';
import { UserStoryService } from '@user-to-story/services/multiple-services-merged/user-story.service';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { StoryModel } from '@stories/models/domain/story-model';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';

@Component({
  selector: 'app-search-story-resume-reading',
  standalone: true,
  imports: [CommonModule, SearchStoryResultsComponent],
  templateUrl: './search-story-resume-reading.component.html',
  styleUrls: ['./search-story-resume-reading.component.scss'],
  providers: [PaginationService],
})
export class SearchStoryResumeReadingComponent implements AfterViewInit {
  private userStoryService = inject(UserStoryService);
  pagination = inject(PaginationService);

  stories = signal<QueriedModel<StoryModel>>(generateEmptyQueriedResult());
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
}
