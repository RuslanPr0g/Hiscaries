import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { PublishStoryRequest } from '@stories/models/requests/publish-story.model';
import { GenreModel } from '@stories/models/domain/genre.model';
import { StoryModel, StoryModelWithContents } from '@stories/models/domain/story-model';
import { SearchStoryRequest } from '@stories/models/requests/search-story.model';
import { ModifyStoryRequest } from '@stories/models/requests/modify-story.model';
import { SearchStoryWithContentsRequest } from '@stories/models/requests/search-story-with-contents.model';
import { SearchStoryByIdsRequest } from '@stories/models/requests/story-by-ids.model';
import { SearchStoryByLibraryRequest } from '@stories/models/requests/search-story-by-library.model';
import { QueriedModel } from '@shared/models/queried.model';
import { BaseIdModel } from '@shared/models/base-id.model';
import { QueryableModel } from '@shared/models/queryable.model';

@Injectable({
  providedIn: 'root',
})
export class StoryService {
  private apiUrl = `${environment.apiUrl}/stories`;

  constructor(private http: HttpClient) {}

  genreList(): Observable<GenreModel[]> {
    return this.http.get<GenreModel[]>(`${this.apiUrl}/genres`);
  }

  recommendations(request: QueryableModel): Observable<QueriedModel<StoryModel>> {
    return this.http.post<QueriedModel<StoryModel>>(`${this.apiUrl}/recommendations`, request);
  }

  getStoriesByLibrary(request: SearchStoryByLibraryRequest): Observable<QueriedModel<StoryModel>> {
    return this.http.post<QueriedModel<StoryModel>>(`${this.apiUrl}/libraries/search`, request);
  }

  searchStory(request: SearchStoryRequest): Observable<QueriedModel<StoryModel>> {
    return this.http.post<QueriedModel<StoryModel>>(`${this.apiUrl}/search`, request);
  }

  getStoryByIdWithContents(
    request: SearchStoryWithContentsRequest,
  ): Observable<StoryModelWithContents> {
    return this.http.post<StoryModelWithContents>(`${this.apiUrl}/by-id-with-contents`, request);
  }

  getStoriesByIds(request: SearchStoryByIdsRequest): Observable<QueriedModel<StoryModel>> {
    return this.http.post<QueriedModel<StoryModel>>(`${this.apiUrl}/by-ids`, request);
  }

  publish(request: PublishStoryRequest): Observable<BaseIdModel> {
    return this.http.post<BaseIdModel>(`${this.apiUrl}`, request);
  }

  modify(request: ModifyStoryRequest): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}`, request);
  }
}
