export interface UserReadingStoryRequest {
  Items: UserReadingStoryRequestItem[];
}

interface UserReadingStoryRequestItem {
  StoryId: string;
  LibraryId: string;
  TotalPages: number;
}
