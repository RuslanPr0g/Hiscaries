import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LibraryModel } from '@users/models/domain/library.model';
import { SocialMediaIconMapperService } from '@shared/services/social-media-icon-mapper.service';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ModifyLibraryFormModel } from '@users/models/form/modify-library.model';
import { FormTextareaComponent } from '@shared/components/form-textarea/form-textarea.component';
import { UploadFileControlComponent } from '@shared/components/upload-file-control/upload-file-control.component';
import { ChipsModule } from 'primeng/chips';

@Component({
  selector: 'app-library-general-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ChipsModule,
    FormTextareaComponent,
    UploadFileControlComponent,
  ],
  templateUrl: './library-general-edit.component.html',
  styleUrl: './library-general-edit.component.scss',
})
export class LibraryGeneralEditComponent implements OnInit {
  modifyForm: FormGroup<ModifyLibraryFormModel>;

  @Input() library: LibraryModel;
  @Input() isAbleToEdit = false;

  @Output() editCancelled = new EventEmitter<void>();
  @Output() editSaved = new EventEmitter<LibraryModel>();

  constructor(private iconService: SocialMediaIconMapperService, private fb: FormBuilder) {}

  ngOnInit(): void {
    if (this.library) {
      this.modifyForm = this.fb.group<ModifyLibraryFormModel>({
        Bio: this.fb.control<string | null>(this.library.Bio),
        AvatarUrl: this.fb.control<string | null>(this.library.AvatarUrl),
        LinksToSocialMedia: this.fb.control<string[] | null>(this.library.LinksToSocialMedia),
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
    this.editCancelled?.emit();
  }

  saveEdit(model: LibraryModel): void {
    this.editSaved?.emit(model);
  }

  onSubmit(): void {
    const formValue = this.modifyForm.value;
    this.saveEdit({
      ...this.library,
      // TODO: figure out what to do in these kinda situations
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      ...(formValue as any),
    });
  }
}
