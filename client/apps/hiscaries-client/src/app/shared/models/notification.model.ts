export interface NotificationModel {
  Id: string;
  UserId: string;
  Message: string;
  IsRead: boolean;
  Type: string;
  RelatedObjectId?: string;
  PreviewUrl?: string;
}
