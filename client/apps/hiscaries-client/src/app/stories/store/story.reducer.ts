import { InitialStoryStateModel } from './story-state.model';
import {
  resolvePdfConflict,
  resolvePdfConflictFailure,
  resolvePdfConflictSuccess,
  savePdfAnnotations,
  savePdfAnnotationsFailure,
  savePdfAnnotationsSuccess,
  searchStoryByTerm,
} from './story.actions';
import { createReducer, on } from '@ngrx/store';

export const storyFeatureKey = 'story';

export const storyReducer = createReducer(
  InitialStoryStateModel,
  on(searchStoryByTerm, (state, story) => {
    return { ...state, SearchTerm: story.SearchTerm };
  }),
  on(savePdfAnnotations, (state) => {
    return { ...state, PdfSaving: true, PdfSaveError: null };
  }),
  on(savePdfAnnotationsSuccess, (state) => {
    return { ...state, PdfSaving: false };
  }),
  on(savePdfAnnotationsFailure, (state, { error }) => {
    return { ...state, PdfSaving: false, PdfSaveError: error };
  }),
  on(resolvePdfConflict, (state) => {
    return { ...state, PdfSaving: true, PdfSaveError: null };
  }),
  on(resolvePdfConflictSuccess, (state) => {
    return { ...state, PdfSaving: false };
  }),
  on(resolvePdfConflictFailure, (state, { error }) => {
    return { ...state, PdfSaving: false, PdfSaveError: error };
  }),
);
