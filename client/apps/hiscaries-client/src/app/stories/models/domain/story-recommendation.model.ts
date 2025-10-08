export interface StoryRecommendationModel {
  Id: string;
  Title: string;
  Description: string;
  Genres: string[];
  LibraryId: string;
  PublishedDate: Date;
  UniqueReads: number;
}
