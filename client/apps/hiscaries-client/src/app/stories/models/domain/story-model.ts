import { GenreModel } from './genre.model';

export interface StoryModel {
  Id: string;
  Title: string;
  Description: string;
  AuthorName?: string;
  AgeLimit: number;
  ImagePreviewUrl: string;
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
