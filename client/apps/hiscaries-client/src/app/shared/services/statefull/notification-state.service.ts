import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { NotificationModel } from '../../models/notification.model';

@Injectable({
  providedIn: 'root',
})
export class NotificationStateService {
  private unreadNotificationsCount = new BehaviorSubject<number>(0);
  private notifications = new BehaviorSubject<NotificationModel[]>([]);
  private readNotifications = new Subject<NotificationModel[]>();

  unreadCount$ = this.unreadNotificationsCount.asObservable();
  notifications$ = this.notifications.asObservable();
  notificationMarkedAsRead$ = this.readNotifications.asObservable();

  addNotification(notification: NotificationModel): void {
    const currentNotifications = this.notifications.value;

    if (currentNotifications.some((x) => x.Id === notification.Id)) {
      return;
    }

    this.notifications.next([notification, ...currentNotifications]);

    const currentUnreadCount = this.unreadNotificationsCount.value;
    this.unreadNotificationsCount.next(currentUnreadCount + 1);
  }

  setNotifications(notifications: NotificationModel[]): void {
    this.notifications.next(notifications);

    const currentUnreadCount = notifications.filter((x) => !x.IsRead).length;
    this.unreadNotificationsCount.next(currentUnreadCount);
  }

  markAllAsRead(): void {
    this.unreadNotificationsCount.next(0);
    this.readNotifications.next(this.notifications.value.filter((x) => !x.IsRead));
  }

  markNotificationsAsRead(notificationsToRead: NotificationModel[]): void {
    this.readNotifications.next(notificationsToRead);
  }
}
