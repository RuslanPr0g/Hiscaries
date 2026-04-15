import { selectPdfSaving } from '../store/story.selector';
import { ReadStoryContentComponent } from './read-story-content.component';
import { Injectable, inject } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { CanDeactivate } from '@angular/router';
import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { Observable, of } from 'rxjs';
import { filter, map, take } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class PdfSaveGuard implements CanDeactivate<ReadStoryContentComponent> {
  private store = inject(Store);
  private messageService = inject(MessageService);
  private pdfSaving = this.store.selectSignal(selectPdfSaving);

  canDeactivate(): Observable<boolean> {
    const pdfSaving = this.pdfSaving();

    if (!pdfSaving) {
      return of(true);
    }

    this.messageService.add({
      key: 'guard-blocking',
      severity: 'info',
      summary: 'Saving your annotations...',
      detail: 'Please wait while we save your PDF annotations.',
      sticky: true,
    });

    return toObservable(this.pdfSaving).pipe(
      filter((saving) => !saving),
      take(1),
      map(() => {
        this.messageService.clear('guard-blocking');
        return true;
      }),
    );
  }
}
