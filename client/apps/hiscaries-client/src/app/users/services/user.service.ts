import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '@environments/environment';
import { LastReadAtDateToId } from '@shared/models/last-read-date-id.model';
import { LibraryModel } from '@users/models/domain/library.model';
import { EditLibraryRequest } from '@users/models/requests/edit-library.model';
import { LibrarySubscriptionRequest } from '@users/models/requests/library-subscription.model';
import { ReadStoryRequest } from '@users/models/requests/read-story.model';
import { UserReadingStoryRequest } from '@users/models/requests/user-reading-story.model';
import { UserReadingStoryMetadataResponse } from '@users/models/response/user-reading-story-metadata.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/users`;

  getUserReadingStoryMetadata(
    request: UserReadingStoryRequest,
  ): Observable<UserReadingStoryMetadataResponse[]> {
    return this.http.post<UserReadingStoryMetadataResponse[]>(
      this.apiUrl + '/story-metadata',
      request,
    );
  }

  becomePublisher(): Observable<void> {
    return this.http.post<void>(this.apiUrl + '/become-publisher', {});
  }

  getLibrary(libraryId?: string): Observable<LibraryModel> {
    if (libraryId) {
      const params = new HttpParams().set('libraryId', libraryId);
      return this.http.get<LibraryModel>(this.apiUrl + '/libraries', { params: params });
    }

    return this.http.get<LibraryModel>(this.apiUrl + '/libraries');
  }

  editLibrary(request: EditLibraryRequest): Observable<void> {
    return this.http.put<void>(this.apiUrl + '/libraries', request);
  }

  subscribeToLibrary(request: LibrarySubscriptionRequest): Observable<void> {
    return this.http.post<void>(this.apiUrl + '/libraries/subscriptions/subscribe', request);
  }

  unsubscribeFromLibrary(request: LibrarySubscriptionRequest): Observable<void> {
    return this.http.post<void>(this.apiUrl + '/libraries/subscriptions/unsubscribe', request);
  }

  read(request: ReadStoryRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/read`, request);
  }

  resumeReading(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/resume-reading`);
  }

  readingHistory(): Observable<LastReadAtDateToId[]> {
    return this.http.get<LastReadAtDateToId[]>(`${this.apiUrl}/reading-history`);
  }

  resolveAnnotatedPdfConflict(storyId: string, keepMine: boolean): Observable<void> {
    const params = new HttpParams().set('storyId', storyId).set('keepMine', keepMine.toString());
    return this.http.post<void>(`${this.apiUrl}/annotated-pdf/resolve-conflict`, null, { params });
  }
}
