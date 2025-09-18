import { ImageUrlSizes } from '@shared/models/image-url-sizes.model';
import { UserModel } from '@users/models/domain/user.model';

export interface LibraryModel {
  PlatformUser: UserModel;
  Id: string;
  Bio: string;
  AvatarImageUrls: ImageUrlSizes;
  LinksToSocialMedia: string[];

  IsLibraryOwner: boolean;
  IsSubscribed: boolean;
  SubscribersCount: number;
}
