import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '@environments/environment';
import { QueriedModel } from '@shared/models/queried.model';
import { QueryableModel } from '@shared/models/queryable.model';
import { StoryRecommendationModel } from '@stories/models/domain/story-recommendation.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StoryRecommendationService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/recommendations`;

  recommendations(request: QueryableModel): Observable<QueriedModel<StoryRecommendationModel>> {
    return this.http.post<QueriedModel<StoryRecommendationModel>>(
      `${this.apiUrl}/stories`,
      request,
    );
  }
}
