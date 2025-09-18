import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { StoryModel } from '@stories/models/domain/story-model';
import { SearchStoryItemComponent } from '@stories/story-search-item/story-search-item.component';
import { CarouselModule, CarouselResponsiveOptions } from 'primeng/carousel';
import { SkeletonOrStoryContentComponent } from '@stories/load-story-or-content/skeleton-or-story-content.component';
import { QueriedModel } from '@shared/models/queried.model';

@Component({
  selector: 'app-search-story-results',
  standalone: true,
  imports: [
    CommonModule,
    SearchStoryItemComponent,
    CarouselModule,
    SkeletonOrStoryContentComponent,
  ],
  templateUrl: './search-story-results.component.html',
  styleUrls: ['./search-story-results.component.scss'],
})
export class SearchStoryResultsComponent implements OnInit {
  @Input() stories: QueriedModel<StoryModel>;
  @Input() isLoading: boolean | null = true;
  @Input() isCarousel = false;

  responsiveOptions: CarouselResponsiveOptions[] | undefined;

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
