import { ImageUrlSizes } from '@shared/models/image-url-sizes.model';
import { GenreModel } from '@stories/models/domain/genre.model';

export interface StoryModel {
  Id: string;
  Title: string;
  Description: string;
  AuthorName?: string;
  AgeLimit: number;
  ImagePreviewUrl: ImageUrlSizes;
  DatePublished: Date;
  DateWritten: Date;
  IsEditable: boolean;
  PercentageRead: number;
  LastPageRead: number;
  TotalPages: number;

  LibraryId: string;
  LibraryName: string;
}

export interface StoryModelWithContents extends StoryModel {
  Genres: GenreModel[];
  Contents: StoryPageModel[];
}

export interface StoryPageModel {
  Page: number;
  Content: string;
}

export interface ReadStoryContentModel {
  Title: string;
  ImagePreviewUrl?: string;
  Contents: StoryPageModel[];
  LastPageRead: number;
}

export interface ReadHistoryStory extends StoryModel {
  LastReadAt: Date;
}
