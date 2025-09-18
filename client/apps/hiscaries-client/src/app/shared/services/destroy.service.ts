import { Injectable, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class DestroyService implements OnDestroy {
  private _subject$ = new Subject<void>();

  get subject$(): Subject<void> {
    return this._subject$;
  }

  ngOnDestroy(): void {
    this._subject$.next();
    this._subject$.complete();
  }
}
