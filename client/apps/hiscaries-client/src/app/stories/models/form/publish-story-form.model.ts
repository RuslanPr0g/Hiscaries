import { AbstractControl } from '@angular/forms';
import { GenreModel } from '@stories/models/domain/genre.model';

export interface PublishFormModel {
    Title: AbstractControl<string | null>;
    Description: AbstractControl<string | null>;
    AuthorName: AbstractControl<string | null>;
    Image: AbstractControl<string | null>;
    Genres: AbstractControl<GenreModel[] | null>;
    AgeLimit: AbstractControl<number | null>;
    DateWritten: AbstractControl<Date | null>;
}
