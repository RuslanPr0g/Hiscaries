import { SearchStoryResultsComponent } from './search-story-results.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { QueriedModel } from '@shared/models/queried.model';
import { StoryModel, StoryStatus } from '@stories/models/domain/story-model';

const mockStory = (overrides: Partial<StoryModel> = {}): StoryModel => ({
  Id: '1',
  Title: 'Test Story',
  Description: 'A test description',
  AuthorName: 'Author One',
  AgeLimit: 12,
  ImagePreviewUrl: null as unknown as string,
  GenreNames: [],
  DatePublished: new Date(),
  DateWritten: new Date(),
  IsEditable: false,
  PercentageRead: 50,
  LastPageRead: 1,
  TotalPages: 10,
  LibraryId: 'lib1',
  LibraryName: 'Library',
  HasExternalPdf: false,
  Status: StoryStatus.Active,
  ...overrides,
});

describe('SearchStoryResultsComponent', () => {
  let fixture: ComponentFixture<SearchStoryResultsComponent>;
  let component: SearchStoryResultsComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchStoryResultsComponent],
      providers: [{ provide: Router, useValue: { navigate: jest.fn() } }],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(SearchStoryResultsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  describe('when isLoading is true and stories is null', () => {
    it('should pass isLoading=true to ui-card-grid', () => {
      fixture.componentRef.setInput('isLoading', true);
      fixture.componentRef.setInput('stories', null);
      fixture.detectChanges();

      const cardGrid = fixture.nativeElement.querySelector('ui-card-grid');
      expect(cardGrid).toBeTruthy();
    });

    it('should not render any ui-media-card elements', () => {
      fixture.componentRef.setInput('isLoading', true);
      fixture.componentRef.setInput('stories', null);
      fixture.detectChanges();

      const mediaCards = fixture.nativeElement.querySelectorAll('ui-media-card');
      expect(mediaCards.length).toBe(0);
    });
  });

  describe('when stories has items', () => {
    let stories: QueriedModel<StoryModel>;

    beforeEach(() => {
      stories = {
        Items: [
          mockStory({ Id: '1', Title: 'Story One' }),
          mockStory({ Id: '2', Title: 'Story Two' }),
        ],
        TotalItemsCount: 2,
      };
      fixture.componentRef.setInput('isLoading', false);
      fixture.componentRef.setInput('stories', stories);
      fixture.detectChanges();
    });

    it('should render one ui-media-card per story item', () => {
      const mediaCards = fixture.nativeElement.querySelectorAll('ui-media-card');
      expect(mediaCards.length).toBe(2);
    });

    it('should render ui-card-grid', () => {
      const cardGrid = fixture.nativeElement.querySelector('ui-card-grid');
      expect(cardGrid).toBeTruthy();
    });
  });

  describe('when stories is empty', () => {
    it('should not render any ui-media-card elements', () => {
      fixture.componentRef.setInput('isLoading', false);
      fixture.componentRef.setInput('stories', { Items: [], TotalItemsCount: 0 });
      fixture.detectChanges();

      const mediaCards = fixture.nativeElement.querySelectorAll('ui-media-card');
      expect(mediaCards.length).toBe(0);
    });
  });
});
