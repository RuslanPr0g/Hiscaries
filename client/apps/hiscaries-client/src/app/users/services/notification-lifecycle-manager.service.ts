import { AuthService } from './auth.service';
import { UserRealTimeNotificationService } from './real-time-notification.service';
import { Injectable, inject } from '@angular/core';
import { NotificationHandler } from '@shared/models/notification-handler.model';

@Injectable({
  providedIn: 'root',
})
export class NotificationLifecycleManagerService {
  private authService = inject(AuthService);
  private realTimeNotificationService = inject(UserRealTimeNotificationService);

  private isInitialized = false;

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
