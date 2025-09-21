export interface QueriedModel<T> {
  Items: T[];
  TotalItemsCount: number;
}

export const generateEmptyQueriedResult = <T>(): QueriedModel<T> => ({
  Items: [],
  TotalItemsCount: 0,
});
