import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
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
import { DividerModule } from 'primeng/divider';
import { GenreModel } from '@stories/models/domain/genre.model';
import { take } from 'rxjs';
import { UploadFileControlComponent } from '@shared/components/upload-file-control/upload-file-control.component';
import { FormMultiselectComponent } from '@shared/components/form-multiselect/form-multiselect.component';
import { MessageModule } from 'primeng/message';
import { ActivatedRoute, Router } from '@angular/router';
import { ModifyStoryFormModel } from '@stories/models/form/modify-story-form.model';
import { ModifyStoryRequest } from '@stories/models/requests/modify-story.model';
import { StoryModelWithContents } from '@stories/models/domain/story-model';
import { NavigationConst } from '@shared/constants/navigation.const';
import { AuthService } from '@users/services/auth.service';
import { ContentBuilderComponent } from './content-builder/content-builder.component';
import { TabViewModule } from 'primeng/tabview';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';

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
    DividerModule,
    UploadFileControlComponent,
    FormMultiselectComponent,
    MessageModule,
    ContentBuilderComponent,
    TabViewModule,
    LoadingSpinnerComponent,
  ],
  templateUrl: './modify-story.component.html',
  styleUrls: ['./modify-story.component.scss'],
})
export class ModifyStoryComponent implements OnInit {
  private storyId: string | null = null;

  modifyForm: FormGroup<ModifyStoryFormModel>;
  genres: GenreModel[] = [];
  submitted = false;
  globalError: string | null = null;

  story: StoryModelWithContents | null = null;
  storyNotFound = false;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private storyService: StoryWithMetadataService,
    private userService: AuthService,
    private router: Router,
  ) {
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
      Contents: this.fb.array<string[]>([], Validators.required),
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

          this.populateFormWithValue();
        },
        error: () => (this.storyNotFound = true),
      });
  }

  get imageControl(): AbstractControl<string | null, string | null> | null {
    return this.modifyForm.get('Image');
  }

  get base64Image(): string | null | undefined {
    return this.imageControl?.value;
  }

  get contents(): FormArray {
    return this.modifyForm.get('Contents') as FormArray;
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

    var isValidPreview = isValidImageBase64Selected(formModel.Image as any);

    const request: ModifyStoryRequest = {
      ...formModel,
      GenreIds: formModel.Genres?.map((g) => g.Id),
      ImagePreview: isValidPreview ? formModel.Image : null,
      StoryId: this.storyId,
      Contents: formModel.Contents?.filter((c: string) => !!c) ?? [],
      ShouldUpdatePreview: isValidPreview,
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
}
