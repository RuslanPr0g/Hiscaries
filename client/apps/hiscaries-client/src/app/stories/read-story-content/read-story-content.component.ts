import { Component, OnInit, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReadStoryContentModel } from '@stories/models/domain/story-model';
import { CommonModule } from '@angular/common';
import { take } from 'rxjs';
import { convertToBase64 } from '@shared/helpers/image.helper';
import { IteratorService } from '@shared/services/statefull/iterator/iterator.service';
import { ButtonModule } from 'primeng/button';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { UserService } from '@users/services/user.service';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-read-story-content',
  standalone: true,
  imports: [CommonModule, ButtonModule, LoadingSpinnerComponent],
  providers: [IteratorService],
  templateUrl: './read-story-content.component.html',
  styleUrl: './read-story-content.component.scss',
})
export class ReadStoryContentComponent implements OnInit {
  storyId: string | null = null;

  globalError: string | null = null;
  story: ReadStoryContentModel | null = null;
  storyNotFound = false;

  maximized = false;

  constructor(
    private route: ActivatedRoute,
    private storyService: StoryWithMetadataService,
    private userService: UserService,
    private iterator: IteratorService,
  ) {
    this.storyId = this.route.snapshot.paramMap.get('id');
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
            this.iterator.moveTo(story.LastPageRead);
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
    if (event.key === 'ArrowRight' || event.key === 'ArrowUp' || event.key === 'Enter') {
      this.moveNext();
    } else if (
      event.key === 'Backspace' ||
      event.key === 'ArrowLeft' ||
      event.key === 'ArrowDown'
    ) {
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
    const result = this.iterator.moveNext();

    if (result && this.storyId && this.iterator.currentIndex > (this.story?.LastPageRead ?? 0)) {
      this.userService
        .read({
          StoryId: this.storyId,
          PageRead: this.currentIndex,
        })
        .subscribe();
    }

    return result;
  }

  movePrev(): boolean {
    return this.iterator.movePrev();
  }

  maximize(): void {
    this.maximized = true;
  }

  minimize(): void {
    this.maximized = false;
  }
}
