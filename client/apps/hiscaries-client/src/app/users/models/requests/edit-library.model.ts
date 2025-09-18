export interface EditLibraryRequest {
  LibraryId: string;
  Bio: string;
  Avatar: string | null;
  ShouldUpdateAvatar: boolean;
  LinksToSocialMedia: string[];
}
