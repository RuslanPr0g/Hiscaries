import {
  Component,
  OnInit,
  AfterViewInit,
  inject,
  signal,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { DestroyService } from '@shared/services/destroy.service';
import { StoryModel } from '@stories/models/domain/story-model';
import { SearchStoryResultsComponent } from '@stories/search-story-results/search-story-results.component';
import { TemplateMessageModel } from '@stories/models/template-message.model';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';
import { PaginationService } from '@shared/services/statefull/pagination.service';

@Component({
  selector: 'app-search-story',
  standalone: true,
  imports: [CommonModule, SearchStoryResultsComponent],
  templateUrl: './search-story.component.html',
  styleUrls: ['./search-story.component.scss'],
  providers: [DestroyService, PaginationService],
})
export class SearchStoryComponent implements OnInit, AfterViewInit {
  private storyService = inject(StoryWithMetadataService);
  private pagination = inject(PaginationService);
  private route = inject(ActivatedRoute);

  @ViewChild('loadMoreAnchor', { static: true }) loadMoreAnchor!: ElementRef<HTMLDivElement>;
  private observer!: IntersectionObserver;

  stories = signal<QueriedModel<StoryModel>>(generateEmptyQueriedResult());
  errorMessage = signal<TemplateMessageModel | null>(null);
  isLoading = signal(false);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const term = params.get('term');
      if (term) {
        this.loadStories(term, true);
      }
    });
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

  private loadStories(term: string, reset = false) {
    if (this.pagination.snapshot.StartIndex > 300) {
      return;
    }

    if (reset) {
      this.pagination.reset();
      this.stories.set(generateEmptyQueriedResult());
    }

    this.isLoading.set(true);

    this.storyService
      .searchStory({ SearchTerm: term, QueryableModel: this.pagination.snapshot })
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe((data) => {
        if (!data?.Items || data.Items.length === 0) {
          this.observer.unobserve(this.loadMoreAnchor.nativeElement);
          return;
        }

        const current = reset ? generateEmptyQueriedResult<StoryModel>() : this.stories();
        this.stories.set({
          Items: [...current.Items, ...data.Items],
          TotalItemsCount: data.TotalItemsCount,
        });

        this.errorMessage.set(
          data.Items.length === 0 && this.stories().Items.length === 0
            ? { Title: 'Search criteria', Description: 'No stories found by the criteria' }
            : null,
        );
      });
  }

  nextPage() {
    const term = this.route.snapshot.paramMap.get('term');
    if (!term) return;
    this.pagination.nextPage();
    this.loadStories(term);
  }

  prevPage() {
    const term = this.route.snapshot.paramMap.get('term');
    if (!term) return;
    this.pagination.prevPage();
    this.loadStories(term);
  }

  get storiesLoaded(): boolean {
    return this.stories().Items.length > 0;
  }
}
