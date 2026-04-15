import { StoryStateModel } from './story-state.model';
import { createSelector } from '@ngrx/store';

export const searchTerm = (state: StoryStateModel) => state.SearchTerm;

export const searchSearchTerm = createSelector(searchTerm, (state: string | null) => state);

export const selectPdfSaving = createSelector(
  (state: StoryStateModel) => state.PdfSaving,
  (saving) => saving,
);

export const selectPdfSaveError = createSelector(
  (state: StoryStateModel) => state.PdfSaveError,
  (error) => error,
);
