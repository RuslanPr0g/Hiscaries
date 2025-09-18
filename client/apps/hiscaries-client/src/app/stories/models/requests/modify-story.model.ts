import { BaseStoryModificationRequest } from './base-story-modification.model';

export interface ModifyStoryRequest extends BaseStoryModificationRequest {
  StoryId: string;
  Contents?: string[];
  ShouldUpdatePreview: boolean;
}
