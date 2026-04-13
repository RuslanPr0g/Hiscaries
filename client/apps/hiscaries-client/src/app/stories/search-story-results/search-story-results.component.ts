
import { Component, OnInit, input } from '@angular/core';
import { StoryModel } from '@stories/models/domain/story-model';
import { SearchStoryItemComponent } from '@stories/story-search-item/story-search-item.component';
import { SkeletonOrStoryContentComponent } from '@stories/load-story-or-content/skeleton-or-story-content.component';
import { QueriedModel } from '@shared/models/queried.model';

@Component({
  selector: 'app-search-story-results',
  standalone: true,
  imports: [
    SearchStoryItemComponent,
    SkeletonOrStoryContentComponent
  ],
  templateUrl: './search-story-results.component.html',
  styleUrls: ['./search-story-results.component.scss'],
})
export class SearchStoryResultsComponent implements OnInit {
  readonly stories = input<QueriedModel<StoryModel>>();
  readonly isLoading = input<boolean | null>(true);
  readonly isCarousel = input(false);

  responsiveOptions: { breakpoint: string; numVisible: number; numScroll: number }[] | undefined;

  ngOnInit(): void {
    this.initializeResponsiveOptions();
  }

  private initializeResponsiveOptions(): void {
    this.responsiveOptions = [
      {
        breakpoint: '1950px',
        numVisible: 3,
        numScroll: 2,
      },
      {
        breakpoint: '1150px',
        numVisible: 2,
        numScroll: 1,
      },
      {
        breakpoint: '767px',
        numVisible: 1,
        numScroll: 1,
      },
    ];
  }
}
