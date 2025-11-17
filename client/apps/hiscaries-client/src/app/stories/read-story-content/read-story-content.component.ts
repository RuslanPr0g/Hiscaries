import { Component, OnInit, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReadStoryContentModel } from '@stories/models/domain/story-model';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject, switchMap, take } from 'rxjs';
import { convertToBase64 } from '@shared/helpers/image.helper';
import { IteratorService } from '@shared/services/statefull/iterator/iterator.service';
import { ButtonModule } from 'primeng/button';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { UserService } from '@users/services/user.service';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';
import { ReadStoryRequest } from '@users/models/requests/read-story.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-read-story-content',
  standalone: true,
  imports: [CommonModule, ButtonModule, LoadingSpinnerComponent, FormsModule],
  providers: [IteratorService],
  templateUrl: './read-story-content.component.html',
  styleUrl: './read-story-content.component.scss',
})
export class ReadStoryContentComponent implements OnInit {
  storyId: string | null = null;

  private pageRead$ = new Subject<ReadStoryRequest>();

  globalError: string | null = null;
  story: ReadStoryContentModel | null = null;
  storyNotFound = false;

  maximized = false;

  pageInput: number = 1;

  constructor(
    private route: ActivatedRoute,
    private storyService: StoryWithMetadataService,
    private userService: UserService,
    private iterator: IteratorService,
  ) {
    this.storyId = this.route.snapshot.paramMap.get('id');

    this.pageRead$
      .pipe(
        debounceTime(500),
        distinctUntilChanged((a, b) => a.StoryId === b.StoryId && a.PageRead === b.PageRead),
        switchMap((payload) => this.userService.read(payload)),
      )
      .subscribe();
  }

  ngOnInit() {
    if (!this.storyId) {
      this.storyNotFound = true;
      return;
    }

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

          this.story = {
            ...story,
            ImagePreviewUrl: imageUrl ? convertToBase64(imageUrl) : undefined,
          };

          if (story.LastPageRead) {
            this.pageInput = story.LastPageRead;
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

  get currentPageLabel(): string {
    return `Page: ${this.currentIndex + 1} / ${this.contents.length}`;
  }

  moveNext(): boolean {
    const moved = this.iterator.moveNext();

    if (moved && this.storyId && this.iterator.currentIndex > (this.story?.LastPageRead ?? 0)) {
      this.pageRead$.next({
        StoryId: this.storyId,
        PageRead: this.currentIndex,
      });
    }

    if (moved) {
      this.pageInput = this.currentIndex + 1;
    }

    return moved;
  }

  movePrev(): boolean {
    const moved = this.iterator.movePrev();
    if (moved) this.pageInput = this.currentIndex + 1;
    return moved;
  }

  goToPage() {
    const page = this.pageInput - 1;

    if (page >= 0 && page < this.contents.length) {
      this.iterator.moveTo(page);
    } else {
      this.pageInput = this.iterator.currentIndex + 1;
    }
  }

  maximize(): void {
    this.maximized = true;
  }

  minimize(): void {
    this.maximized = false;
  }
}
