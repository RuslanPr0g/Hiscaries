import { UserModel } from './user.model';

export interface LibraryModel {
  PlatformUser: UserModel;
  Id: string;
  Bio: string;
  AvatarUrl: string;
  LinksToSocialMedia: string[];

  IsLibraryOwner: boolean;
  IsSubscribed: boolean;
  SubscribersCount: number;
}
