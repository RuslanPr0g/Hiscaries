import { QueryableModel } from '@shared/models/queryable.model';

export interface SearchStoryByIdsRequest {
  Ids: string[];
  QueryableModel?: QueryableModel;
}
