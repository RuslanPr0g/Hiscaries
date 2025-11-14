import { AbstractControl, FormArray, FormControl } from '@angular/forms';
import { GenreModel } from '@stories/models/domain/genre.model';

export interface ModifyStoryFormModel {
  Title: AbstractControl<string | null>;
  Description: AbstractControl<string | null>;
  AuthorName: AbstractControl<string | null>;
  Image: AbstractControl<string | null>;
  Genres: AbstractControl<GenreModel[] | null>;
  AgeLimit: AbstractControl<number | null>;
  DateWritten: AbstractControl<Date | null>;
  Contents: FormArray<FormControl<string | null>>;
  PdfFile: AbstractControl<string | null>;
}
