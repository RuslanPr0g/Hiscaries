import { Injectable } from '@angular/core';
import { Observable, switchMap, map, catchError, of } from 'rxjs';
import { BaseIdModel } from '@shared/models/base-id.model';
import { GenreModel } from '@stories/models/domain/genre.model';
import { StoryModel, StoryModelWithContents } from '@stories/models/domain/story-model';
import { ModifyStoryRequest } from '@stories/models/requests/modify-story.model';
import { PublishStoryRequest } from '@stories/models/requests/publish-story.model';
import { SearchStoryWithContentsRequest } from '@stories/models/requests/search-story-with-contents.model';
import { SearchStoryRequest } from '@stories/models/requests/search-story.model';
import { StoryService } from '@stories/services/story.service';
import { UserService } from '@users/services/user.service';
import { SearchStoryByIdsRequest } from '@stories/models/requests/story-by-ids.model';
import { QueriedModel } from '@shared/models/queried.model';
import { SearchStoryByLibraryRequest } from '@stories/models/requests/search-story-by-library.model';
import { QueryableModel } from '@shared/models/queryable.model';

@Injectable({
  providedIn: 'root',
})
export class StoryWithMetadataService {
  constructor(private storyService: StoryService, private userService: UserService) {}

  genreList(): Observable<GenreModel[]> {
    return this.storyService.genreList();
  }

  recommendations(request: QueryableModel): Observable<QueriedModel<StoryModel>> {
    return this.enrichStories(this.storyService.recommendations(request));
  }

  getStoriesByLibrary(request: SearchStoryByLibraryRequest): Observable<QueriedModel<StoryModel>> {
    return this.enrichStories(this.storyService.getStoriesByLibrary(request));
  }

  searchStory(request: SearchStoryRequest): Observable<QueriedModel<StoryModel>> {
    return this.enrichStories(this.storyService.searchStory(request));
  }

  getStoryByIdWithContents(
    request: SearchStoryWithContentsRequest,
  ): Observable<StoryModelWithContents> {
    return this.enrichStory(this.storyService.getStoryByIdWithContents(request));
  }

  getStoriesByIds(request: SearchStoryByIdsRequest): Observable<QueriedModel<StoryModel>> {
    return this.enrichStories(this.storyService.getStoriesByIds(request));
  }

  publish(request: PublishStoryRequest): Observable<BaseIdModel> {
    return this.storyService.publish(request);
  }

  modify(request: ModifyStoryRequest): Observable<void> {
    return this.storyService.modify(request);
  }

  private enrichStories(
    source$: Observable<QueriedModel<StoryModel>>,
  ): Observable<QueriedModel<StoryModel>> {
    return this.enrich(source$) as Observable<QueriedModel<StoryModel>>;
  }

  private enrichStory(
    source$: Observable<StoryModelWithContents>,
  ): Observable<StoryModelWithContents> {
    return this.enrich(source$) as Observable<StoryModelWithContents>;
  }

  private enrich<T extends StoryModel>(
    source$: Observable<T | QueriedModel<T>>,
  ): Observable<T | QueriedModel<T> | null> {
    return source$.pipe(
      switchMap((input) => {
        if (!input) {
          return of(input);
        }

        const isQueriedModel =
          (input as QueriedModel<T>).Items !== undefined &&
          (input as QueriedModel<T>).TotalItemsCount !== undefined;

        if (isQueriedModel && (input as QueriedModel<T>).Items.length === 0) {
          return of(input);
        }

        const stories = isQueriedModel ? (input as QueriedModel<T>).Items : [input as T];

        const metadataRequest = {
          Items: isQueriedModel
            ? (input as QueriedModel<T>).Items.map((story) => ({
                StoryId: story.Id,
                LibraryId: story.LibraryId,
                TotalPages: story.TotalPages,
              }))
            : [
                {
                  StoryId: (input as T).Id,
                  LibraryId: (input as T).LibraryId,
                  TotalPages: (input as T).TotalPages,
                },
              ],
        };

        return this.userService.getUserReadingStoryMetadata(metadataRequest).pipe(
          map((metadataList) => {
            if (!metadataList || metadataList.length === 0) {
              return input;
            }

            const metadataMap = new Map(metadataList.map((meta) => [meta.StoryId, meta]));
            const enriched = stories.map((story) => {
              const metadata = metadataMap.get(story.Id);
              return metadata ? { ...story, ...metadata } : story;
            });

            if (isQueriedModel) {
              return {
                ...input,
                Items: enriched,
              };
            }

            return enriched[0];
          }),
          catchError((error) => {
            console.error('Failed to fetch story metadata', error);
            return of(input);
          }),
        );
      }),
    );
  }
}
