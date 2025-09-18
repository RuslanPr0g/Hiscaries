import { ImageUrlSizes } from "@shared/models/image-url-sizes.model";

export interface NotificationModel {
  Id: string;
  UserId: string;
  Message: string;
  IsRead: boolean;
  Type: string;
  RelatedObjectId?: string;
  ImageUrls?: ImageUrlSizes;
}
