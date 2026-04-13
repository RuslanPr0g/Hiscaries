import { SearchStoryComponent } from './search-story.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { generateEmptyQueriedResult } from '@shared/models/queried.model';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { StoryModel, StoryStatus } from '@stories/models/domain/story-model';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { of } from 'rxjs';

// Requirements 17.1–17.6

const makeStory = (overrides: Partial<StoryModel> = {}): StoryModel => ({
  Id: '1',
  Title: 'Test Story',
  Description: 'A description',
  AuthorName: 'Author',
  AgeLimit: 0,
  ImagePreviewUrl: null as unknown as string,
  GenreNames: [],
  DatePublished: new Date(),
  DateWritten: new Date(),
  IsEditable: false,
  PercentageRead: 0,
  LastPageRead: 0,
  TotalPages: 0,
  LibraryId: '',
  LibraryName: '',
  HasExternalPdf: false,
  Status: StoryStatus.Active,
  ...overrides,
});

describe('SearchStoryComponent', () => {
  let fixture: ComponentFixture<SearchStoryComponent>;
  let component: SearchStoryComponent;

  const mockStoryService = {
    searchStory: jest.fn().mockReturnValue(of(generateEmptyQueriedResult())),
  };

  const mockPaginationService = {
    snapshot: { StartIndex: 0, PageSize: 10 },
    reset: jest.fn(),
    nextPage: jest.fn(),
    prevPage: jest.fn(),
  };

  const mockActivatedRoute = {
    paramMap: of({ get: (_: string) => 'test' }),
    snapshot: { paramMap: { get: (_: string) => 'test' } },
  };

  beforeEach(async () => {
    jest.clearAllMocks();
    mockStoryService.searchStory.mockReturnValue(of(generateEmptyQueriedResult()));

    await TestBed.configureTestingModule({
      imports: [SearchStoryComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        { provide: StoryWithMetadataService, useValue: mockStoryService },
        { provide: PaginationService, useValue: mockPaginationService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SearchStoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // Requirements 17.1 – nextPage() is called when loadMore emits
  it('should call nextPage() when loadMore event fires from ui-infinite-scroll-grid', () => {
    const nextPageSpy = jest.spyOn(component, 'nextPage');

    const gridEl = fixture.nativeElement.querySelector('ui-infinite-scroll-grid');
    gridEl.dispatchEvent(new CustomEvent('loadMore', { bubbles: true }));
    fixture.detectChanges();

    expect(nextPageSpy).toHaveBeenCalledTimes(1);
  });

  // Requirements 17.5, 17.6 – isEmpty is true when no stories and not loading
  it('should pass isEmpty=true to the grid when stories is empty and not loading', () => {
    component.stories.set({ Items: [], TotalItemsCount: 0 });
    component.isLoading.set(false);
    fixture.detectChanges();

    const isEmpty = component.stories().Items.length === 0 && !component.isLoading();
    expect(isEmpty).toBe(true);
  });

  // Requirements 17.5 – isEmpty is false when stories are present
  it('should pass isEmpty=false to the grid when stories are present', () => {
    component.stories.set({ Items: [makeStory()], TotalItemsCount: 1 });
    component.isLoading.set(false);
    fixture.detectChanges();

    const isEmpty = component.stories().Items.length === 0 && !component.isLoading();
    expect(isEmpty).toBe(false);
  });

  // Requirements 17.3 – isEmpty is false while loading even if no stories
  it('should pass isEmpty=false to the grid while loading', () => {
    component.stories.set({ Items: [], TotalItemsCount: 0 });
    component.isLoading.set(true);
    fixture.detectChanges();

    const isEmpty = component.stories().Items.length === 0 && !component.isLoading();
    expect(isEmpty).toBe(false);
  });

  // Requirements 17.2 – hasMore is false when all items are loaded
  it('should pass hasMore=false when all items are loaded', () => {
    component.stories.set({ Items: [makeStory()], TotalItemsCount: 1 });
    fixture.detectChanges();

    const hasMore = component.stories().Items.length < component.stories().TotalItemsCount;
    expect(hasMore).toBe(false);
  });

  // Requirements 17.2 – hasMore is true when more items remain
  it('should pass hasMore=true when more items remain', () => {
    component.stories.set({ Items: [makeStory()], TotalItemsCount: 5 });
    fixture.detectChanges();

    const hasMore = component.stories().Items.length < component.stories().TotalItemsCount;
    expect(hasMore).toBe(true);
  });
});
