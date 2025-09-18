import { Injectable } from '@angular/core';
import { QueryableModel } from '@shared/models/queryable.model';

@Injectable()
export class PaginationService {
  state: QueryableModel = {
    StartIndex: 0,
    ItemsCount: 15,
    SortProperty: 'CreatedAt',
    SortAsc: false,
  };

  isLoading = false;

  get snapshot() {
    return this.state;
  }

  setPage(start: number, count: number) {
    this.state.StartIndex = start;
    this.state.ItemsCount = count;
  }

  setSort(property: string, asc: boolean) {
    this.state.SortProperty = property;
    this.state.SortAsc = asc;
  }

  nextPage() {
    this.setPage(this.state.StartIndex + this.state.ItemsCount, this.state.ItemsCount);
  }

  prevPage() {
    this.setPage(Math.max(0, this.state.StartIndex - this.state.ItemsCount), this.state.ItemsCount);
  }

  reset() {
    this.state = { StartIndex: 0, ItemsCount: 15, SortProperty: 'CreatedAt', SortAsc: false };
  }

  setLoading(v: boolean) {
    this.isLoading = v;
  }
}
