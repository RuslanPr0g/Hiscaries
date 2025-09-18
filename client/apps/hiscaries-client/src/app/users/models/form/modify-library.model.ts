import { AbstractControl } from '@angular/forms';

export interface ModifyLibraryFormModel {
  Bio: AbstractControl<string | null>;
  AvatarUrl: AbstractControl<string | null>;
  LinksToSocialMedia: AbstractControl<string[] | null>;
}
