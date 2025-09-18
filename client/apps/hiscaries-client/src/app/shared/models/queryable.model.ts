export interface QueryableModel {
  StartIndex: number;
  ItemsCount: number;
  SortProperty: string;
  SortAsc: boolean;
}

export const defaultQueryableModel: QueryableModel = {
  StartIndex: 0,
  ItemsCount: 1,
  SortProperty: 'CreatedAt',
  SortAsc: false,
};
