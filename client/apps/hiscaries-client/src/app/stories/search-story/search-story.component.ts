import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MediaCardComponent } from '@shared/components/molecules/media-card/media-card.component';
import { SectionHeaderComponent } from '@shared/components/molecules/section-header/section-header.component';
import { InfiniteScrollGridComponent } from '@shared/components/organisms/infinite-scroll-grid/infinite-scroll-grid.component';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';
import { DestroyService } from '@shared/services/destroy.service';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { StoryModel } from '@stories/models/domain/story-model';
import { TemplateMessageModel } from '@stories/models/template-message.model';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-search-story',
  standalone: true,
  imports: [
    InfiniteScrollGridComponent,
    MediaCardComponent,
    SectionHeaderComponent,
    FallbackImagePipe,
  ],
  templateUrl: './search-story.component.html',
  styleUrls: ['./search-story.component.scss'],
  providers: [DestroyService, PaginationService],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchStoryComponent implements OnInit {
  private storyService = inject(StoryWithMetadataService);
  private pagination = inject(PaginationService);
  private route = inject(ActivatedRoute);

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
