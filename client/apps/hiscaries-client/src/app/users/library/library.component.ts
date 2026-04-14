import { LibraryGeneralEditComponent } from './library-general-edit/library-general-edit.component';
import { LibraryGeneralViewComponent } from './library-general-view/library-general-view.component';
import { Component, output, input } from '@angular/core';
import { QueriedModel } from '@shared/models/queried.model';
import { StoryModel } from '@stories/models/domain/story-model';
import { SearchStoryResultsComponent } from '@stories/search-story-results/search-story-results.component';
import { LibraryModel } from '@users/models/domain/library.model';
import { ModifyLibraryModel } from '@users/models/domain/modify-library.model';

@Component({
  selector: 'app-library',
  standalone: true,
  imports: [SearchStoryResultsComponent, LibraryGeneralViewComponent, LibraryGeneralEditComponent],
  templateUrl: './library.component.html',
  styleUrl: './library.component.scss',
})
export class LibraryComponent {
  readonly library = input<LibraryModel>();
  readonly stories = input<QueriedModel<StoryModel>>();
  readonly isLoading = input<boolean | null>(false);
  readonly isAbleToEdit = input(false);

  readonly isAbleToSubscribe = input(false);
  readonly isSubscribed = input(false);

  readonly isSubscribeLoading = input(false);

  readonly libraryEdited = output<ModifyLibraryModel>();

  readonly subscribed = output<void>();
  readonly unSubscribed = output<void>();

  isEditMode = false;

  startEdit(): void {
    this.isEditMode = true;
  }

  cancelEdit(): void {
    this.isEditMode = false;
  }

  saveEdit(model: ModifyLibraryModel): void {
    this.libraryEdited?.emit(model);
    this.isEditMode = false;
  }

  subscribeAction(): void {
    // TODO: The 'emit' function requires a mandatory void argument
    this.subscribed?.emit();
  }

  unSubscribeAction(): void {
    // TODO: The 'emit' function requires a mandatory void argument
    this.unSubscribed?.emit();
  }
}
