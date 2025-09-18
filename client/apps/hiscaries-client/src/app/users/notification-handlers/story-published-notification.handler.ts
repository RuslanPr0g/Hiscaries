import { NotificationHandler } from '@shared/models/notification-handler.model';

export class StoryPublishedHandler implements NotificationHandler {
  handleNotification<T>(eventType: string, payload: T): void {
    console.debug(
      `Notification handler ${eventType} received a message with the following payload: ${payload}`,
    );
  }
}
