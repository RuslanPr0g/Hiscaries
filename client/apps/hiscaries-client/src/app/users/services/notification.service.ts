import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { NotificationModel } from '@shared/models/notification.model';
import { ReadNotificationsRequest } from '@users/models/requests/read-notifications.model';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private apiUrl = `${environment.apiUrl}/notifications`;

  constructor(private http: HttpClient) {}

  notifications(): Observable<NotificationModel[]> {
    return this.http.get<NotificationModel[]>(this.apiUrl);
  }

  readNotifications(request: ReadNotificationsRequest): Observable<void> {
    return this.http.post<void>(this.apiUrl, request);
  }
}
