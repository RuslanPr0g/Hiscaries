export interface NotificationHandler {
  handleNotification<T>(eventType: string, payload: T): void;
}
