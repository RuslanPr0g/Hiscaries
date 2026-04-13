import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, output, input } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { FormTextareaComponent } from '@shared/components/form-textarea/form-textarea.component';
import { UploadFileControlComponent } from '@shared/components/upload-file-control/upload-file-control.component';
import { SocialMediaIconMapperService } from '@shared/services/social-media-icon-mapper.service';
import { LibraryModel } from '@users/models/domain/library.model';
import { ModifyLibraryModel } from '@users/models/domain/modify-library.model';
import { ModifyLibraryFormModel } from '@users/models/form/modify-library.model';
import { AutoComplete } from 'primeng/autocomplete';

@Component({
  selector: 'app-library-general-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AutoComplete,
    FormTextareaComponent,
    UploadFileControlComponent,
  ],
  templateUrl: './library-general-edit.component.html',
  styleUrls: ['./library-general-edit.component.scss'],
})
export class LibraryGeneralEditComponent implements OnInit {
  private iconService = inject(SocialMediaIconMapperService);
  private fb = inject(FormBuilder);

  modifyForm: FormGroup<ModifyLibraryFormModel>;

  readonly library = input<LibraryModel>();
  readonly isAbleToEdit = input(false);

  readonly editCancelled = output<void>();
  readonly editSaved = output<ModifyLibraryModel>();

  ngOnInit(): void {
    const library = this.library();
    if (library) {
      const imageUrl =
        library.AvatarImageUrls?.Large ??
        library.AvatarImageUrls?.Medium ??
        library.AvatarImageUrls?.Small ??
        null;
      this.modifyForm = this.fb.group<ModifyLibraryFormModel>({
        Bio: this.fb.control<string | null>(library.Bio),
        AvatarUrl: this.fb.control<string | null>(imageUrl),
        LinksToSocialMedia: this.fb.control<string[] | null>(library.LinksToSocialMedia),
      });
    }
  }

  get backgroundImageUrl(): string | null | undefined {
    return this.imageControl?.value;
  }

  get imageControl(): AbstractControl<string | null, string | null> | null {
    return this.modifyForm.get('AvatarUrl');
  }

  getSocialNetworkIcon(link: string): string {
    return this.iconService.mapFromUrl(link);
  }

  cancelEdit(): void {
    // TODO: The 'emit' function requires a mandatory void argument
    this.editCancelled?.emit();
  }

  saveEdit(model: ModifyLibraryModel): void {
    this.editSaved?.emit(model);
  }

  onSubmit(): void {
    const formValue = this.modifyForm.value;
    this.saveEdit({
      ...this.library(),
      // TODO: figure out what to do in these kinda situations
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      ...(formValue as any),
    });
  }
}
