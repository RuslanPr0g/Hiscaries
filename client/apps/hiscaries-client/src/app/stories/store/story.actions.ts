import { createAction, props } from '@ngrx/store';

export const searchStoryByTerm = createAction(
  '[Story] Search a story by term',
  props<{ SearchTerm: string | null }>(),
);

export const savePdfAnnotations = createAction(
  '[Story] Save PDF Annotations',
  props<{ storyId: string; blob: Blob }>(),
);

export const savePdfAnnotationsSuccess = createAction('[Story] Save PDF Annotations Success');

export const savePdfAnnotationsFailure = createAction(
  '[Story] Save PDF Annotations Failure',
  props<{ error: string }>(),
);

export const resolvePdfConflict = createAction(
  '[Story] Resolve PDF Conflict',
  props<{ storyId: string; keepMine: boolean }>(),
);

export const resolvePdfConflictSuccess = createAction('[Story] Resolve PDF Conflict Success');

export const resolvePdfConflictFailure = createAction(
  '[Story] Resolve PDF Conflict Failure',
  props<{ error: string }>(),
);
