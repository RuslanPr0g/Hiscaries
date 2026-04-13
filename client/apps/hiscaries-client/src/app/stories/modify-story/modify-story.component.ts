import { Component, inject, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormInputComponent } from '@shared/components/form-input/form-input.component';
import { FormTextareaComponent } from '@shared/components/form-textarea/form-textarea.component';
import { ButtonTwoComponent } from '@shared/components/button-two/button-two.component';
import { NumberLimitControlComponent } from '@shared/components/number-limit-control/number-limit-control.component';
import { FormDateInputComponent } from '@shared/components/form-date-input/form-date-input.component';
import { Divider } from 'primeng/divider';
import { GenreModel } from '@stories/models/domain/genre.model';
import { take } from 'rxjs';
import { UploadFileControlComponent } from '@shared/components/upload-file-control/upload-file-control.component';
import { FormMultiselectComponent } from '@shared/components/form-multiselect/form-multiselect.component';
import { Message } from 'primeng/message';
import { ActivatedRoute, Router } from '@angular/router';
import { ModifyStoryFormModel } from '@stories/models/form/modify-story-form.model';
import { ModifyStoryRequest } from '@stories/models/requests/modify-story.model';
import { StoryModelWithContents } from '@stories/models/domain/story-model';
import { NavigationConst } from '@shared/constants/navigation.const';
import { AuthService } from '@users/services/auth.service';
import { ContentBuilderComponent } from './content-builder/content-builder.component';
import { Tabs, Tab, TabList, TabPanel, TabPanels } from 'primeng/tabs';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';
import { DocumentContent } from '@media/models/document-content.model';
import { MediaService } from '@media/services/media.service';
import { OperationResult } from '@shared/models/operation-result.model';

@Component({
  selector: 'app-modify-story',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormInputComponent,
    FormTextareaComponent,
    ButtonTwoComponent,
    FormDateInputComponent,
    NumberLimitControlComponent,
    Divider,
    UploadFileControlComponent,
    FormMultiselectComponent,
    Message,
    ContentBuilderComponent,
    Tabs,
    Tab,
    TabList,
    TabPanel,
    TabPanels,
    LoadingSpinnerComponent,
  ],
  templateUrl: './modify-story.component.html',
  styleUrls: ['./modify-story.component.scss'],
})
export class ModifyStoryComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private storyService = inject(StoryWithMetadataService);
  private userService = inject(AuthService);
  private router = inject(Router);

  private storyId: string | null = null;

  modifyForm: FormGroup<ModifyStoryFormModel>;
  genres: GenreModel[] = [];
  submitted = false;
  globalError: string | null = null;

  story: StoryModelWithContents | null = null;
  storyNotFound = false;

  mediaService = inject(MediaService);

  pdfUrl: string | null = null;
  pdfExists = true;
  uploadedPdfFileName?: string | null = null;

  constructor() {
    this.modifyForm = this.fb.group<ModifyStoryFormModel>({
      Title: this.fb.control<string | null>(null, Validators.required),
      Description: this.fb.control<string | null>(null, Validators.required),
      AuthorName: this.fb.control<string | null>(null, Validators.required),
      Image: this.fb.control<string | null>(null, Validators.required),
      Genres: this.fb.control<GenreModel[] | null>(null, Validators.required),
      AgeLimit: this.fb.control<number | null>(null, [
        Validators.required,
        Validators.min(0),
        Validators.max(18),
      ]),
      DateWritten: this.fb.control<Date | null>(null, Validators.required),
      Contents: this.fb.array<FormControl<string>>([], Validators.required),
      PdfFile: this.fb.control<string | null>(null),
      PdfFileAsStory: this.fb.control<string | null>(null),
    });

    this.storyId = this.route.snapshot.paramMap.get('id');
  }

  ngOnInit() {
    if (!this.storyId) {
      this.storyNotFound = true;
      return;
    }

    this.storyService
      .genreList()
      .pipe(take(1))
      .subscribe((genres: GenreModel[]) => {
        this.genres = genres;
      });

    this.pdfControl?.valueChanges.subscribe((pdfBase64) => {
      if (!pdfBase64) return;

      // TODO: for now only pdf
      const isBase64Pdf = pdfBase64.startsWith('data:application/pdf;base64,');
      if (!isBase64Pdf) {
        console.warn('Selected file is not a PDF.');
        return;
      }

      this.submitted = true;

      const byteString = pdfBase64.split(',')[1];
      const blob = this.base64ToBlob(byteString, 'application/pdf');
      const file = new File([blob], 'uploaded.pdf', { type: 'application/pdf' });

      this.mediaService
        .asContents(file)
        .pipe(take(1))
        .subscribe({
          next: (documentContent: DocumentContent) => {
            const newContents = this.fb.array(
              documentContent.Pages.filter((page) => !!page).map((page) =>
                this.fb.control(page.Text, Validators.required),
              ),
            ) as FormArray<FormControl<string>>;
            this.modifyForm.setControl('Contents', newContents);
            this.submitted = false;
            this.pdfControl?.reset();
          },
          error: (err) => {
            console.error('PDF parsing failed', err);
            this.submitted = false;
          },
        });
    });

    this.pdfAsStoryControl?.valueChanges.subscribe((pdfBase64) => {
      if (!pdfBase64 || !this.storyId) return;

      const isBase64Pdf = pdfBase64.startsWith('data:application/pdf;base64,');
      if (!isBase64Pdf) {
        console.warn('Selected file is not a PDF.');
        return;
      }

      this.submitted = true;

      const byteString = pdfBase64.split(',')[1];
      const blob = this.base64ToBlob(byteString, 'application/pdf');
      const pdfFileName = `${this.storyId}.pdf`;
      const file = new File([blob], pdfFileName, { type: 'application/pdf' });

      this.mediaService
        .upload(this.storyId, file)
        .pipe(take(1))
        .subscribe({
          next: (result: OperationResult) => {
            if (result.ResultStatus != 0) {
              console.error('PDF upload failed');
              this.submitted = false;
              this.pdfAsStoryControl?.reset();
              return;
            } else {
              this.uploadedPdfFileName = pdfFileName;
              this.onSubmit();
              return;
            }
          },
          error: (err) => {
            console.error('PDF upload failed', err);
            this.submitted = false;
            this.pdfAsStoryControl?.reset();
          },
        });
    });

    this.storyService
      .getStoryByIdWithContents({
        Id: this.storyId,
      })
      .pipe(take(1))
      .subscribe({
        next: (story) => {
          if (!story) {
            this.storyNotFound = true;
            return;
          }

          // TODO: fix this it sohuld no be username but id
          if (!this.userService.isTokenOwnerByUsername(story.LibraryName)) {
            this.storyNotFound = true;
            console.warn('User is not a publisher of this story.');
            this.router.navigate([NavigationConst.Home]);
            return;
          }

          this.story = story;

          this.checkPdfExists(this.storyId, story.ExternalPdfUrl);

          this.populateFormWithValue();
        },
        error: () => (this.storyNotFound = true),
      });
  }

  get imageControl(): AbstractControl<string | null, string | null> | null {
    return this.modifyForm.get('Image');
  }

  get pdfControl(): AbstractControl<string | null, string | null> | null {
    return this.modifyForm.get('PdfFile');
  }

  get pdfAsStoryControl(): AbstractControl<string | null, string | null> | null {
    return this.modifyForm.get('PdfFileAsStory');
  }

  get base64Image(): string | null | undefined {
    return this.imageControl?.value;
  }

  get contents(): FormArray {
    return this.modifyForm.get('Contents') as FormArray;
  }

  removePdf() {
    if (!this.storyId) return;

    this.submitted = true;

    this.mediaService
      .delete(this.storyId)
      .pipe(take(1))
      .subscribe({
        next: (result: OperationResult) => {
          if (result.ResultStatus != 0) {
            console.error('PDF deletion failed');
            this.submitted = false;
            return;
          }

          this.pdfExists = false;
          this.pdfUrl = null;
          this.submitted = false;
        },
        error: (err) => {
          console.error('PDF deletion failed', err);
          this.submitted = false;
        },
      });
  }

  private checkPdfExists(storyId: string | null, url?: string) {
    if (!url || !storyId) {
      this.pdfExists = false;
      return;
    }

    fetch(url, { method: 'GET' })
      .then((res) => {
        if (res.ok) {
          this.pdfExists = true;
          this.pdfUrl = url;
          this.uploadedPdfFileName = `${storyId}.pdf`;
        } else {
          this.pdfExists = false;
        }
      })
      .catch(() => {
        this.pdfExists = false;
      });
  }

  updateContent(index: number, value: string) {
    const contentsArray = this.modifyForm.get('Contents') as FormArray;
    contentsArray.at(index).setValue(value);
  }

  onSubmit() {
    const isValidImageBase64Selected = (value: string): boolean => {
      if (!value || value.startsWith('data')) {
        return true;
      }

      return false;
    };

    if (!this.storyId) {
      this.storyNotFound = true;
      return;
    }

    if (!this.modifyForm.valid) {
      this.modifyForm.markAllAsTouched();
      return;
    }

    this.submitted = true;

    const formModel = this.modifyForm.value;

    const isValidPreview = isValidImageBase64Selected(formModel.Image as string);

    const request: ModifyStoryRequest = {
      ...formModel,
      GenreIds: formModel.Genres?.map((g) => g.Id),
      ImagePreview: isValidPreview ? formModel.Image : null,
      StoryId: this.storyId,
      Contents: formModel.Contents?.filter((c) => !!c) ?? [],
      ShouldUpdatePreview: isValidPreview,
      PdfFileName: this.uploadedPdfFileName,
    };

    this.storyService
      .modify(request)
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.router.navigate([NavigationConst.PreviewStory(this.storyId!)]);
        },
        error: (error) => {
          if (error) {
            this.globalError = 'Error occured while modifying the story, please try again later';
          }

          this.submitted = false;
        },
      });
  }

  private populateFormWithValue(): void {
    const imageUrl =
      this.story?.ImagePreviewUrl?.Large ??
      this.story?.ImagePreviewUrl?.Medium ??
      this.story?.ImagePreviewUrl?.Small;

    if (this.story) {
      this.modifyForm.patchValue({
        Title: this.story.Title,
        Description: this.story.Description,
        AuthorName: this.story.AuthorName,
        Image: imageUrl,
        Genres: this.story.Genres,
        AgeLimit: this.story.AgeLimit,
        DateWritten: new Date(this.story.DateWritten),
      });

      const contentsArray = this.modifyForm.get('Contents') as FormArray;
      this.story.Contents.forEach((x) => {
        contentsArray.push(this.fb.control(x.Content));
      });
    }
  }

  private base64ToBlob(base64: string, contentType: string): Blob {
    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    return new Blob([byteArray], { type: contentType });
  }
}
