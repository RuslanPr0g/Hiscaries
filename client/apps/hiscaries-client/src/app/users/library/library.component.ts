import { Component, EventEmitter, Input, Output } from '@angular/core';
import { LibraryModel } from '@users/models/domain/library.model';
import { StoryModel } from '@stories/models/domain/story-model';
import { CommonModule } from '@angular/common';
import { SearchStoryResultsComponent } from '@stories/search-story-results/search-story-results.component';
import { LibraryGeneralViewComponent } from './library-general-view/library-general-view.component';
import { LibraryGeneralEditComponent } from './library-general-edit/library-general-edit.component';
import { QueriedModel } from '@shared/models/queried.model';
import { ModifyLibraryModel } from '@users/models/domain/modify-library.model';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-library',
  standalone: true,
  imports: [
    CommonModule,
    SearchStoryResultsComponent,
    LibraryGeneralViewComponent,
    LibraryGeneralEditComponent,
    LoadingSpinnerComponent,
  ],
  templateUrl: './library.component.html',
  styleUrl: './library.component.scss',
})
export class LibraryComponent {
  @Input() library: LibraryModel;
  @Input() stories: QueriedModel<StoryModel>;
  @Input() isLoading: boolean | null = false;
  @Input() isAbleToEdit = false;

  @Input() isAbleToSubscribe = false;
  @Input() isSubscribed = false;

  @Input() isSubscribeLoading = false;

  @Output() libraryEdited = new EventEmitter<ModifyLibraryModel>();

  @Output() subscribed = new EventEmitter<void>();
  @Output() unSubscribed = new EventEmitter<void>();

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
    this.subscribed?.emit();
  }

  unSubscribeAction(): void {
    this.unSubscribed?.emit();
  }
}
