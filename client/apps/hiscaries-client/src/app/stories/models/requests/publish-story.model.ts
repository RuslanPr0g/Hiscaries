import { BaseStoryModificationRequest } from './base-story-modification.model';

export interface PublishStoryRequest extends BaseStoryModificationRequest {
  LibraryId: string;
}
