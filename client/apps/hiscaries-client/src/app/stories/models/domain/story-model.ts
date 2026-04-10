import { ImageUrlSizes } from '@shared/models/image-url-sizes.model';
import { GenreModel } from '@stories/models/domain/genre.model';

export interface StoryModel {
  Id: string;
  Title: string;
  Description: string;
  AuthorName?: string;
  AgeLimit: number;
  ImagePreviewUrl: ImageUrlSizes;
  GenreNames: string[];
  DatePublished: Date;
  DateWritten: Date;
  IsEditable: boolean;
  PercentageRead: number;
  LastPageRead: number;
  TotalPages: number;

  LibraryId: string;
  LibraryName: string;

  HasExternalPdf: boolean;
  ExternalPdfUrl?: string;
  ExternalPdfSize?: number;
  ExternalPdfPageCount?: number;
  Status: StoryStatus;
}

export enum StoryStatus {
  Draft = 0,
  Active = 1,
  Deleted = 2,
  ConsolidatingDocuments = 3
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
