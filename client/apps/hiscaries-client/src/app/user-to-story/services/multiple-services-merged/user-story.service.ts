import { Injectable } from '@angular/core';
import { Observable, of, switchMap, catchError, map } from 'rxjs';
import { ReadHistoryStory, StoryModel } from '@stories/models/domain/story-model';
import { UserService } from '@users/services/user.service';
import { StoryWithMetadataService } from './story-with-metadata.service';
import { QueryableModel } from '@shared/models/queryable.model';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';
import { LastReadAtDateToId } from '@shared/models/last-read-date-id.model';

@Injectable({
  providedIn: 'root',
})
export class UserStoryService {
  constructor(private storyService: StoryWithMetadataService, private userService: UserService) {}

  resumeReading(queryableModel: QueryableModel): Observable<QueriedModel<StoryModel>> {
    return this.loadStoriesFromIds(() => this.userService.resumeReading(), {
      ...queryableModel,
      SortProperty: 'LastPageRead',
      SortAsc: false,
    });
  }

  readingHistory(queryableModel: QueryableModel): Observable<QueriedModel<ReadHistoryStory>> {
    return this.loadStoriesHistoryFromIds(() => this.userService.readingHistory(), {
      ...queryableModel,
      SortProperty: 'EditedAt',
      SortAsc: false,
    });
  }

  private loadStoriesHistoryFromIds(
    idsSource: () => Observable<LastReadAtDateToId[]>,
    queryableModel: QueryableModel,
  ): Observable<QueriedModel<ReadHistoryStory>> {
    if (!idsSource) {
      return of(generateEmptyQueriedResult<ReadHistoryStory>());
    }

    return idsSource().pipe(
      switchMap((history): Observable<QueriedModel<ReadHistoryStory>> => {
        if (!Array.isArray(history) || history.length === 0) {
          return of(generateEmptyQueriedResult<ReadHistoryStory>());
        }

        const idToDateMap = new Map<string, Date>(
          history.map((entry) => [entry.Id.toString(), new Date(entry.LastReadAt)]),
        );

        const validIds = Array.from(idToDateMap.keys());

        return this.storyService
          .getStoriesByIds({
            Ids: validIds,
            QueryableModel: queryableModel,
          })
          .pipe(
            map((result): QueriedModel<ReadHistoryStory> => {
              const merged: ReadHistoryStory[] = result.Items.map((story) => ({
                ...story,
                LastReadAt: idToDateMap.get(story.Id) ?? new Date(),
              }));

              merged.sort((a, b) => b.LastReadAt.getTime() - a.LastReadAt.getTime());

              return {
                Items: merged,
                TotalItemsCount: result.TotalItemsCount,
              };
            }),
            catchError(() => of(generateEmptyQueriedResult<ReadHistoryStory>())),
          );
      }),
      catchError(() => of(generateEmptyQueriedResult<ReadHistoryStory>())),
    );
  }

  private loadStoriesFromIds(
    idsSource: () => Observable<string[]>,
    queryableModel: QueryableModel,
  ): Observable<QueriedModel<StoryModel>> {
    if (!idsSource) {
      return of(generateEmptyQueriedResult<StoryModel>());
    }

    try {
      return idsSource().pipe(
        switchMap((ids) => {
          if (
            !Array.isArray(ids) ||
            ids.length === 0 ||
            ids.some((id) => typeof id !== 'string' || !id.trim())
          ) {
            return of(generateEmptyQueriedResult<StoryModel>());
          }

          try {
            return this.storyService
              .getStoriesByIds({
                Ids: ids,
                QueryableModel: queryableModel,
              })
              .pipe(catchError(() => of(generateEmptyQueriedResult<StoryModel>())));
          } catch {
            return of(generateEmptyQueriedResult<StoryModel>());
          }
        }),
        catchError(() => of(generateEmptyQueriedResult<StoryModel>())),
      );
    } catch {
      return of(generateEmptyQueriedResult<StoryModel>());
    }
  }
}
