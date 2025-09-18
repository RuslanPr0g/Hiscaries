import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { NotificationHandler } from '@shared/models/notification-handler.model';
import { UserRealTimeNotificationService } from './real-time-notification.service';

@Injectable({
  providedIn: 'root',
})
export class NotificationLifecycleManagerService {
  private isInitialized = false;

  constructor(
    private authService: AuthService,
    private realTimeNotificationService: UserRealTimeNotificationService,
  ) {}

  initialize(handlers: NotificationHandler[]): void {
    if (this.isInitialized) {
      console.warn('Notification system is already initialized.');
      return;
    }

    if (this.authService.isAuthenticated()) {
      this.realTimeNotificationService.initialize(handlers);
      this.isInitialized = true;
    } else {
      console.warn('User is not authenticated. Notifications will not be initialized.');
    }
  }

  stop(): void {
    this.realTimeNotificationService.disconnect();
    this.isInitialized = false;
  }
}
