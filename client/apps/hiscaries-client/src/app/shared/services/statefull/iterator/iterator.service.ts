import { Injectable } from '@angular/core';

@Injectable()
export class IteratorService {
  private _upperBoundary = 0;
  private _currentIndex = 0;

  set upperBoundary(value: number) {
    this._upperBoundary = value;
  }

  get currentIndex(): number {
    return this._currentIndex;
  }

  moveNext(): boolean {
    if (this._currentIndex === this._upperBoundary) {
      return false;
    }

    this._currentIndex++;

    return true;
  }

  movePrev(): boolean {
    if (this._currentIndex === 0) {
      return false;
    }

    this._currentIndex--;

    return true;
  }

  moveToLast(): boolean {
    return this.moveTo(this._upperBoundary);
  }

  moveToFirst(): boolean {
    return this.moveTo(0);
  }

  moveTo(index: number): boolean {
    this._currentIndex = index;
    return true;
  }
}
