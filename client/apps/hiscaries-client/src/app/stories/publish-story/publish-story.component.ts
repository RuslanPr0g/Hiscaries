import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FormInputComponent } from '@shared/components/form-input/form-input.component';
import { FormTextareaComponent } from '@shared/components/form-textarea/form-textarea.component';
import { FormButtonComponent } from '@shared/components/form-button/form-button.component';
import { NumberLimitControlComponent } from '@shared/components/number-limit-control/number-limit-control.component';
import { FormDateInputComponent } from '@shared/components/form-date-input/form-date-input.component';
import { DividerModule } from 'primeng/divider';
import { GenreModel } from '@stories/models/domain/genre.model';
import { take } from 'rxjs';
import { UploadFileControlComponent } from '@shared/components/upload-file-control/upload-file-control.component';
import { FormMultiselectComponent } from '@shared/components/form-multiselect/form-multiselect.component';
import { PublishFormModel } from '@stories/models/form/publish-story-form.model';
import { PublishStoryRequest } from '@stories/models/requests/publish-story.model';
import { MessageModule } from 'primeng/message';
import { Router } from '@angular/router';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BaseIdModel } from '@shared/models/base-id.model';
import { NavigationConst } from '@shared/constants/navigation.const';
import { AuthService } from '@users/services/auth.service';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { UserService } from '@users/services/user.service';
import { LibraryModel } from '@users/models/domain/library.model';

@Component({
  selector: 'app-publish-story',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormInputComponent,
    FormTextareaComponent,
    FormButtonComponent,
    FormDateInputComponent,
    NumberLimitControlComponent,
    DividerModule,
    UploadFileControlComponent,
    FormMultiselectComponent,
    MessageModule,
    ProgressSpinnerModule,
  ],
  templateUrl: './publish-story.component.html',
  styleUrls: ['./publish-story.component.scss'],
})
export class PublishStoryComponent implements OnInit {
  publishForm: FormGroup<PublishFormModel>;
  genres: GenreModel[] = [];
  library: LibraryModel;
  submitted = false;
  globalError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private storyService: StoryWithMetadataService,
    private router: Router,
    private userService: UserService,
    public authService: AuthService,
  ) {
    if (!this.authService.isPublisher()) {
      this.navigateHome();
      return;
    }

    this.publishForm = this.fb.group<PublishFormModel>({
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
    });
  }

  ngOnInit() {
    this.userService
      .getLibrary()
      .pipe(take(1))
      .subscribe((library: LibraryModel) => {
        if (!library?.Id) {
          this.navigateHome();
        }

        this.library = library;

        this.storyService
          .genreList()
          .pipe(take(1))
          .subscribe((genres: GenreModel[]) => {
            this.genres = genres;
          });
      });
  }

  get imageControl(): AbstractControl<string | null, string | null> | null {
    return this.publishForm.get('Image');
  }

  get base64Image(): string | null | undefined {
    return this.imageControl?.value;
  }

  onSubmit() {
    if (!this.publishForm.valid) {
      this.publishForm.markAllAsTouched();
      return;
    }

    this.submitted = true;

    const formModel = this.publishForm.value;

    const request: PublishStoryRequest = {
      ...formModel,
      GenreIds: formModel.Genres?.map((g) => g.Id),
      ImagePreview: formModel.Image,
      LibraryId: this.library.Id,
    };

    this.storyService
      .publish(request)
      .pipe(take(1))
      .subscribe({
        next: (story: BaseIdModel) => {
          this.router.navigate([NavigationConst.ModifyStory(story.Id)]);
        },
        error: (error) => {
          if (error) {
            this.globalError = 'Error occured while publishing the story, please try again later';
          }

          this.submitted = false;
        },
      });
  }

  private navigateHome() {
    console.warn('User is not a publisher!');
    this.router.navigate([NavigationConst.Home]);
  }
}
