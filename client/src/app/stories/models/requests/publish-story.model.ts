import { GenreModel } from "../domain/genre.model";

export interface PublishStoryRequest {
    Title?: string | null;
    Description?: string | null;
    AuthorName?: string | null;
    GenreIds?: string[] | null;
    AgeLimit?: number | null;
    ImagePreview?: string | null;
    DateWritten?: Date | null;
}