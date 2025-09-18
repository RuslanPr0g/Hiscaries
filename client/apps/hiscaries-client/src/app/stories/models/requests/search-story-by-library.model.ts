import { QueryableModel } from '@shared/models/queryable.model';

export interface SearchStoryByLibraryRequest {
  LibraryId?: string | null;
  QueryableModel?: QueryableModel;
}
