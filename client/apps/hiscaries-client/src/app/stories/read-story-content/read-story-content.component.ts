import { ReadingSettingsComponent } from './reading-settings/reading-settings.component';
import { CommonModule } from '@angular/common';
import {
  Component,
  OnInit,
  OnDestroy,
  HostListener,
  ElementRef,
  ViewChild,
  inject,
  DestroyRef,
  effect,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';
import { convertToBase64 } from '@shared/helpers/image.helper';
import { IteratorService } from '@shared/services/statefull/iterator/iterator.service';
import {
  defaultReadingSettings,
  ReadingSettings,
} from '@stories/models/domain/reading-settings.model';
import { StoryModelWithContents } from '@stories/models/domain/story-model';
import { LoadReadingSettingsService } from '@stories/services/load-reading-settings.service';
import { savePdfAnnotations, resolvePdfConflict } from '@stories/store/story.actions';
import { selectPdfSaving, selectPdfSaveError } from '@stories/store/story.selector';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { ReadStoryRequest } from '@users/models/requests/read-story.model';
import { UserReadingStoryMetadataResponse } from '@users/models/response/user-reading-story-metadata.model';
import { UserService } from '@users/services/user.service';
import { NgxExtendedPdfViewerModule, NgxExtendedPdfViewerService } from 'ngx-extended-pdf-viewer';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { debounceTime, Subject, switchMap, take } from 'rxjs';

@Component({
  selector: 'app-read-story-content',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    NgxExtendedPdfViewerModule,
    LoadingSpinnerComponent,
    FormsModule,
    ToastModule,
    DialogModule,
    ReadingSettingsComponent,
  ],
  providers: [IteratorService, MessageService],
  templateUrl: './read-story-content.component.html',
  styleUrl: './read-story-content.component.scss',
})
export class ReadStoryContentComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private storyService = inject(StoryWithMetadataService);
  private userService = inject(UserService);
  private iterator = inject(IteratorService);
  private store = inject(Store);
  private pdfViewerService = inject(NgxExtendedPdfViewerService);
  private destroyRef = inject(DestroyRef);

  pdfSaving = this.store.selectSignal(selectPdfSaving);
  pdfSaveError = this.store.selectSignal(selectPdfSaveError);

  @ViewChild('contentWrapper') contentWrapper!: ElementRef<HTMLDivElement>;

  private pageRead$ = new Subject<ReadStoryRequest>();
  private pdfAnnotationChange$ = new Subject<Promise<Blob | undefined>>();
  private lastPdfHash: string | null = null;
  private isExportingPdf = false;

  private touchStartX = 0;
  private touchEndX = 0;
  private minSwipeDistance = 110;

  storyId: string | null = null;

  settingsVisible = false;
  messageService = inject(MessageService);
  settingsService = inject(LoadReadingSettingsService);

  settings: ReadingSettings = defaultReadingSettings;

  globalError: string | null = null;
  story: StoryModelWithContents | null = null;
  storyNotFound = false;
  metadata: UserReadingStoryMetadataResponse | null = null;

  maximized = false;

  pageInput = 1;
  pdfPage = 1;

  conflictModalVisible = false;

  pdfLoadAttempt: 'user' | 'story' | 'fallback-to-text' = 'user';

  get pdfUrl(): string | null {
    if (this.pdfLoadAttempt === 'user' && this.metadata?.UserAnnotatedPdfUrl) {
      return this.metadata.UserAnnotatedPdfUrl;
    }
    if (this.pdfLoadAttempt === 'story' && this.story?.ExternalPdfUrl) {
      return this.story.ExternalPdfUrl;
    }
    if (this.pdfLoadAttempt === 'user' && this.story?.ExternalPdfUrl) {
      return this.story.ExternalPdfUrl;
    }
    return null;
  }

  get pdfExists(): boolean {
    return this.story?.HasExternalPdf ?? false;
  }

  get isConsolidatingDocuments(): boolean {
    return this.story?.Status === 3;
  }

  get shouldShowPdf(): boolean {
    if (this.isConsolidatingDocuments) {
      return false;
    }
    return this.settings.PreferPdf && this.pdfExists;
  }

  get preferPdfDisabled(): boolean {
    if (this.shouldShowPdf) {
      return this.pdfPage > this.contents.length;
    }
    return this.pageInput > this.pdfPage;
  }

  constructor() {
    this.storyId = this.route.snapshot.paramMap.get('id');

    this.pageRead$
      .pipe(
        debounceTime(500),
        switchMap((payload) => this.userService.read(payload)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe();

    this.pdfAnnotationChange$
      .pipe(
        debounceTime(2000),
        switchMap(async (blobPromise) => {
          const blob = await blobPromise;
          if (!blob) return null;

          const hash = `${blob.size}-${blob.type}`;
          if (hash === this.lastPdfHash) return null;

          this.lastPdfHash = hash;
          return blob;
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((blob) => {
        if (blob && this.storyId) {
          this.store.dispatch(savePdfAnnotations({ storyId: this.storyId, blob }));
        }
      });

    effect(() => {
      const error = this.pdfSaveError();
      if (error) {
        this.messageService.add({
          key: 'pdf-error',
          severity: 'error',
          summary: 'Failed to save annotations',
          detail: error,
          life: 5000,
        });
      }
    });
  }

  ngOnInit() {
    if (!this.storyId) {
      this.storyNotFound = true;
      return;
    }

    this.pdfLoadAttempt = 'user';

    this.settings = this.settingsService.getSettings();

    this.storyService
      .getStoryByIdWithContents({
        Id: this.storyId,
      })
      .pipe(take(1))
      .subscribe({
        next: (story) => {
          if (!story) {
            this.storyNotFound = true;
            return;
          }

          const imageUrl =
            story.ImagePreviewUrl?.Large ??
            story.ImagePreviewUrl?.Medium ??
            story.ImagePreviewUrl?.Small;

          const storyWithMetadata = story as StoryModelWithContents & {
            UserAnnotatedPdfUrl?: string;
            HasPdfConflict?: boolean;
          };
          this.metadata = {
            StoryId: story.Id,
            LibraryName: story.LibraryName,
            IsEditable: story.IsEditable,
            PercentageRead: story.PercentageRead,
            LastPageRead: story.LastPageRead,
            UserAnnotatedPdfUrl: storyWithMetadata.UserAnnotatedPdfUrl,
            HasPdfConflict: storyWithMetadata.HasPdfConflict,
          };

          this.story = {
            ...story,
            ImagePreviewUrl: imageUrl ? { Large: convertToBase64(imageUrl) } : {},
          };

          if (this.metadata.HasPdfConflict === true) {
            this.conflictModalVisible = true;
          }

          if (story.LastPageRead) {
            this.pageInput = story.LastPageRead;
            this.pdfPage = story.LastPageRead;
            this.goToPage();
          } else {
            this.userService
              .read({
                StoryId: story.Id,
                PageRead: 0,
              })
              .subscribe();
          }

          this.iterator.upperBoundary = story.Contents.length - 1;
        },
        error: () => (this.storyNotFound = true),
      });
  }

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.key === 'ArrowRight' || event.key === 'Enter') {
      this.moveNext();
    } else if (event.key === 'Backspace' || event.key === 'ArrowLeft') {
      this.movePrev();
    } else if (event.key === ' ') {
      this.maximize();
    } else if (event.key === 'Escape') {
      this.minimize();
    }
  }

  get currentIndex(): number {
    return this.iterator.currentIndex;
  }

  get contents(): string[] {
    return this.story?.Contents?.map((contentModel) => contentModel.Content) ?? [];
  }

  get currentPageContent(): string {
    return this.contents.at(this.currentIndex) ?? 'Page is empty';
  }

  onTouchStart(event: TouchEvent) {
    this.touchStartX = event.changedTouches[0].screenX;
  }

  onTouchEnd(event: TouchEvent) {
    this.touchEndX = event.changedTouches[0].screenX;
    this.handleSwipe();
  }

  moveNext(): boolean {
    const moved = this.iterator.moveNext();

    if (moved && this.storyId && this.iterator.currentIndex + 1 > (this.story?.LastPageRead ?? 0)) {
      this.pageRead$.next({
        StoryId: this.storyId,
        PageRead: this.currentIndex + 1,
      });
    }

    if (moved) {
      this.pageInput = this.currentIndex + 1;
      this.pdfPage = this.pageInput;
      this.scrollToTop();
    }

    return moved;
  }

  movePrev(): boolean {
    const moved = this.iterator.movePrev();

    if (moved && this.storyId) {
      this.pageRead$.next({
        StoryId: this.storyId,
        PageRead: this.currentIndex + 1,
      });
    }

    if (moved) {
      this.pageInput = this.currentIndex + 1;
      this.pdfPage = this.pageInput;
      this.scrollToTop();
    }
    return moved;
  }

  goToPage() {
    const page = this.pageInput - 1;

    if (page >= 0 && page < this.contents.length) {
      this.iterator.moveTo(page);
      this.pdfPage = this.pageInput;
      this.scrollToTop();
    } else {
      this.pageInput = this.iterator.currentIndex + 1;
    }
  }

  private onFullscreenChange = () => {
    if (!document.fullscreenElement) {
      this.maximized = false;
    }
  };

  maximize(): void {
    this.maximized = true;
    this.onSettingsClose();
    document.documentElement.requestFullscreen().catch((_err) => {
      // Fullscreen request failed, continue without fullscreen
    });
    document.addEventListener('fullscreenchange', this.onFullscreenChange);
  }

  minimize(): void {
    this.maximized = false;
    if (document.fullscreenElement) {
      document.exitFullscreen().catch((_err) => {
        // Exit fullscreen failed, continue anyway
      });
    }
    document.removeEventListener('fullscreenchange', this.onFullscreenChange);
  }

  ngOnDestroy(): void {
    if (document.fullscreenElement) {
      document.exitFullscreen().catch(() => {
        // Exit fullscreen failed during cleanup
      });
    }
    document.removeEventListener('fullscreenchange', this.onFullscreenChange);
  }

  showSettingss() {
    if (!this.settingsVisible) {
      this.messageService.add({
        key: 'settings',
        sticky: true,
        severity: 'custom',
        summary: 'Settings',
        styleClass: 'narrow-toast backdrop-blur-lg rounded-2xl',
      });
      this.settingsVisible = true;
    }
  }

  onSettingsClose() {
    this.settingsVisible = false;
    this.messageService.clear();
  }

  onSettingsChanged(newSettings: ReadingSettings) {
    const wasShowingPdf = this.shouldShowPdf;
    this.settings = newSettings;
    const isNowShowingPdf = this.shouldShowPdf;

    if (!wasShowingPdf && isNowShowingPdf) {
      this.pdfPage = this.pageInput;
    } else if (wasShowingPdf && !isNowShowingPdf) {
      this.pageInput = this.pdfPage;
      this.iterator.moveTo(this.pdfPage - 1);
    }
  }

  onPdfPageChange(page: number) {
    this.pdfPage = page;
    this.pageInput = page;
    this.iterator.moveTo(page - 1);
    if (this.storyId) {
      this.userService
        .read({
          StoryId: this.storyId,
          PageRead: page,
        })
        .subscribe();
    }
  }

  onKeepMine(): void {
    if (!this.storyId) return;
    this.store.dispatch(resolvePdfConflict({ storyId: this.storyId, keepMine: true }));
    this.conflictModalVisible = false;
  }

  onLoadLatest(): void {
    if (!this.storyId) return;
    this.store.dispatch(resolvePdfConflict({ storyId: this.storyId, keepMine: false }));
    this.conflictModalVisible = false;

    this.storyService
      .getStoryByIdWithContents({ Id: this.storyId })
      .pipe(take(1))
      .subscribe({
        next: (story) => {
          if (story) {
            const storyWithMetadata = story as StoryModelWithContents & {
              UserAnnotatedPdfUrl?: string;
              HasPdfConflict?: boolean;
            };
            this.metadata = {
              StoryId: story.Id,
              LibraryName: story.LibraryName,
              IsEditable: story.IsEditable,
              PercentageRead: story.PercentageRead,
              LastPageRead: story.LastPageRead,
              UserAnnotatedPdfUrl: storyWithMetadata.UserAnnotatedPdfUrl,
              HasPdfConflict: storyWithMetadata.HasPdfConflict,
            };

            this.story = { ...story };
          }
        },
      });
  }

  onPdfLoadError(): void {
    if (this.pdfLoadAttempt === 'user' && this.story?.ExternalPdfUrl) {
      this.pdfLoadAttempt = 'story';
      this.messageService.add({
        severity: 'warn',
        summary: 'Annotated PDF unavailable',
        detail: 'Loading the original story PDF instead.',
        life: 3000,
      });
    } else if (this.pdfLoadAttempt === 'story') {
      this.pdfLoadAttempt = 'fallback-to-text';
      this.settings.PreferPdf = false;
      this.messageService.add({
        severity: 'warn',
        summary: 'PDF unavailable',
        detail: 'Showing text contents instead.',
        life: 3000,
      });
    }
  }

  private handleSwipe() {
    const distance = this.touchEndX - this.touchStartX;

    if (Math.abs(distance) > this.minSwipeDistance) {
      if (distance < 0) {
        this.moveNext();
      } else {
        this.movePrev();
      }
    }
  }

  private scrollToTop() {
    if (this.contentWrapper) {
      this.contentWrapper.nativeElement.scrollTop = 0;
    }

    this.scrollPageToTop();
  }

  private scrollPageToTop() {
    window.scrollTo({ top: 0, left: 0, behavior: 'smooth' });
  }

  onPdfAnnotationChange() {
    if (this.isExportingPdf) return;

    this.isExportingPdf = true;
    this.pdfViewerService
      .getCurrentDocumentAsBlob()
      .then((blob) => {
        this.pdfAnnotationChange$.next(Promise.resolve(blob));
      })
      .catch(() => {
        // Failed to export PDF blob, silently ignore
      })
      .finally(() => {
        this.isExportingPdf = false;
      });
  }
}
