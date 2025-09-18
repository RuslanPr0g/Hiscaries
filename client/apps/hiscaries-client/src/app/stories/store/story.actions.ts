import { createAction, props } from '@ngrx/store';

export const searchStoryByTerm = createAction(
  '[Story] Search a story by term',
  props<{ SearchTerm: string | null }>(),
);
