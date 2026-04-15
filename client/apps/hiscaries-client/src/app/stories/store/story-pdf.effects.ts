import {
  resolvePdfConflict,
  resolvePdfConflictFailure,
  resolvePdfConflictSuccess,
  savePdfAnnotations,
  savePdfAnnotationsFailure,
  savePdfAnnotationsSuccess,
} from './story.actions';
import { Injectable, inject } from '@angular/core';
import { MediaService } from '@media/services/media.service';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { UserService } from '@users/services/user.service';
import { catchError, exhaustMap, map, of } from 'rxjs';

@Injectable()
export class StoryPdfEffects {
  private actions$ = inject(Actions);
  private mediaService = inject(MediaService);
  private userService = inject(UserService);

  savePdfAnnotations$ = createEffect(() =>
    this.actions$.pipe(
      ofType(savePdfAnnotations),
      exhaustMap(({ storyId, blob }) =>
        this.mediaService.uploadUserAnnotatedPdf(storyId, blob).pipe(
          map(() => savePdfAnnotationsSuccess()),
          catchError((error: unknown) =>
            of(
              savePdfAnnotationsFailure({
                error: error instanceof Error ? error.message : String(error),
              }),
            ),
          ),
        ),
      ),
    ),
  );

  resolvePdfConflict$ = createEffect(() =>
    this.actions$.pipe(
      ofType(resolvePdfConflict),
      exhaustMap(({ storyId, keepMine }) =>
        this.userService.resolveAnnotatedPdfConflict(storyId, keepMine).pipe(
          map(() => resolvePdfConflictSuccess()),
          catchError((error: unknown) =>
            of(
              resolvePdfConflictFailure({
                error: error instanceof Error ? error.message : String(error),
              }),
            ),
          ),
        ),
      ),
    ),
  );
}
