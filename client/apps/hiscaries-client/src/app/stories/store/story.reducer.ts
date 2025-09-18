import { createReducer, on } from '@ngrx/store';
import { InitialStoryStateModel } from './story-state.model';
import { searchStoryByTerm } from './story.actions';

export const storyFeatureKey = 'story';

export const storyReducer = createReducer(
  InitialStoryStateModel,
  on(searchStoryByTerm, (state, story) => {
    return { ...state, SearchTerm: story.SearchTerm };
  }),
);
