export interface QueriedModel<T> {
  Items: T[];
  TotalItemsCount: number;
}

export const emptyQueriedResult: QueriedModel<any> = {
  Items: [],
  TotalItemsCount: 0,
};
