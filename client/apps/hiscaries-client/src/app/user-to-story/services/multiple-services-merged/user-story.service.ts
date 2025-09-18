import { Injectable } from '@angular/core';
import { Observable, of, switchMap, catchError } from 'rxjs';
import { StoryModel } from '@stories/models/domain/story-model';
import { UserService } from '@users/services/user.service';
import { StoryWithMetadataService } from './story-with-metadata.service';
import { QueryableModel } from '@shared/models/queryable.model';
import { emptyQueriedResult, QueriedModel } from '@shared/models/queried.model';

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

  readingHistory(queryableModel: QueryableModel): Observable<QueriedModel<StoryModel>> {
    return this.loadStoriesFromIds(() => this.userService.readingHistory(), {
      ...queryableModel,
      SortProperty: 'EditedAt',
      SortAsc: false,
    });
  }

  private loadStoriesFromIds(
    idsSource: () => Observable<string[]>,
    queryableModel: QueryableModel,
  ): Observable<QueriedModel<StoryModel>> {
    if (!idsSource) {
      return of(emptyQueriedResult);
    }

    try {
      return idsSource().pipe(
        switchMap((ids) => {
          if (
            !Array.isArray(ids) ||
            ids.length === 0 ||
            ids.some((id) => typeof id !== 'string' || !id.trim())
          ) {
            return of(emptyQueriedResult);
          }

          try {
            return this.storyService
              .getStoriesByIds({
                Ids: ids,
                QueryableModel: queryableModel,
              })
              .pipe(catchError(() => of(emptyQueriedResult)));
          } catch {
            return of(emptyQueriedResult);
          }
        }),
        catchError(() => of(emptyQueriedResult)),
      );
    } catch {
      return of(emptyQueriedResult);
    }
  }
}
