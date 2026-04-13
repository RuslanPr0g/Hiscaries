import { InitialStoryStateModel } from './story-state.model';
import { searchStoryByTerm } from './story.actions';
import { createReducer, on } from '@ngrx/store';

export const storyFeatureKey = 'story';

export const storyReducer = createReducer(
  InitialStoryStateModel,
  on(searchStoryByTerm, (state, story) => {
    return { ...state, SearchTerm: story.SearchTerm };
  }),
);
