import { QueryableModel } from '@shared/models/queryable.model';

export interface SearchStoryRequest {
  Id?: string | null;
  SearchTerm?: string | null;
  Genre?: string | null;
  QueryableModel: QueryableModel;
}
