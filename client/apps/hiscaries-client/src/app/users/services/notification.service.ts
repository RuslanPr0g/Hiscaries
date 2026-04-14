import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '@environments/environment';
import { NotificationModel } from '@shared/models/notification.model';
import { ReadNotificationsRequest } from '@users/models/requests/read-notifications.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/notifications`;

  notifications(): Observable<NotificationModel[]> {
    return this.http.get<NotificationModel[]>(this.apiUrl);
  }

  readNotifications(request: ReadNotificationsRequest): Observable<void> {
    return this.http.post<void>(this.apiUrl, request);
  }
}
