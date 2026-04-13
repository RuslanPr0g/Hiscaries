import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '@environments/environment';
import { BaseIdModel } from '@shared/models/base-id.model';
import { QueriedModel } from '@shared/models/queried.model';
import { GenreModel } from '@stories/models/domain/genre.model';
import { StoryModel, StoryModelWithContents } from '@stories/models/domain/story-model';
import { ModifyStoryRequest } from '@stories/models/requests/modify-story.model';
import { PublishStoryRequest } from '@stories/models/requests/publish-story.model';
import { SearchStoryByLibraryRequest } from '@stories/models/requests/search-story-by-library.model';
import { SearchStoryWithContentsRequest } from '@stories/models/requests/search-story-with-contents.model';
import { SearchStoryRequest } from '@stories/models/requests/search-story.model';
import { SearchStoryByIdsRequest } from '@stories/models/requests/story-by-ids.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StoryService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/stories`;

  genreList(): Observable<GenreModel[]> {
    return this.http.get<GenreModel[]>(`${this.apiUrl}/genres`);
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
