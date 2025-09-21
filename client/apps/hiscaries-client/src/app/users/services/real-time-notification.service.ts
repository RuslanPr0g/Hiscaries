import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service';
import { SignalRConnectionFactoryService } from '@shared/services/statefull/signalr-connection-factory.service';
import { NotificationHandler } from '@shared/models/notification-handler.model';
import { NotificationStateService } from '@shared/services/statefull/notification-state.service';
import { take } from 'rxjs';
import { NotificationModel } from '@shared/models/notification.model';
import { UserNotificationTypes } from '@shared/constants/notification-type.const';
import { NotificationService } from './notification.service';
import { BaseIdModel } from '@shared/models/base-id.model';

@Injectable({
  providedIn: 'root',
})
export class UserRealTimeNotificationService {
  private notificationHandlers: NotificationHandler[];
  private hubConnection: signalR.HubConnection;
  private readonly hubUrl = '/hubs/usernotifications';

  disconnected = true;

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private connectionFactory: SignalRConnectionFactoryService,
    private notificationStateService: NotificationStateService,
  ) {}

  initialize(handlers: NotificationHandler[]): void {
    const token = this.authService.getToken();

    if (!token) {
      console.warn('Token is missing. Cannot establish SignalR connection.');
      return;
    }

    this.fetchNotificationsAndSetInState();

    this.hubConnection = this.connectionFactory.createSSEConnection(this.hubUrl, token);

    this.registerEventListeners();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connection started successfully.'))
      .catch((err) => console.error('Error starting SignalR connection:', err));

    this.notificationHandlers = handlers;

    this.notificationStateService.notificationMarkedAsRead$.subscribe((notifications) => {
      this.notificationService
        .readNotifications({
          NotificationIds: notifications.map((x: BaseIdModel) => x.Id),
        })
        .pipe(take(1))
        .subscribe(() => {
          this.fetchNotificationsAndSetInState();
        });
    });
  }

  disconnect(): void {
    if (this.hubConnection && !this.disconnected) {
      this.disconnected = true;
      this.hubConnection.stop().then(() => console.log('SignalR connection stopped.'));
    }
  }

  private fetchNotificationsAndSetInState(): void {
    this.notificationService
      .notifications()
      .pipe(take(1))
      .subscribe((notifications) => {
        this.notificationStateService.setNotifications(notifications);
      });
  }

  private registerEventListeners(): void {
    this.registerEventHandlers();

    this.hubConnection.onclose((error) => {
      console.error('SignalR connection closed.', error);
      this.disconnected = true;
    });

    this.hubConnection.onreconnecting((error) => {
      console.warn('SignalR reconnecting...', error);
    });

    this.hubConnection.onreconnected((connectionId) => {
      console.log('SignalR reconnected with connectionId:', connectionId);
      this.disconnected = false;
    });
  }

  private registerEventHandlers(): void {
    this.hubConnection.on(UserNotificationTypes.StoryPublished, (message) =>
      this.dispatchNotification(UserNotificationTypes.StoryPublished, message),
    );
  }

  private dispatchNotification(eventType: string, payload: NotificationModel): void {
    this.notificationHandlers.forEach((handler) => handler.handleNotification(eventType, payload));

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const convertToPascalCase = (obj: any): any =>
      Object.fromEntries(
        Object.entries(obj).map(([key, value]) => [
          key.replace(/^\w/, (c) => c.toUpperCase()),
          value,
        ]),
      );

    this.notificationStateService.addNotification(convertToPascalCase(payload));
  }
}
