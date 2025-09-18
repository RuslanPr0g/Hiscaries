import { UserModel } from "./user.model";

export interface ModifyLibraryModel {
  PlatformUser: UserModel;
  Id: string;
  Bio: string;
  AvatarUrl: string | null;
  LinksToSocialMedia: string[];

  IsLibraryOwner: boolean;
  IsSubscribed: boolean;
  SubscribersCount: number;
}