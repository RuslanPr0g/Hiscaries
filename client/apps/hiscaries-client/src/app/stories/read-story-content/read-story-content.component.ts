import { Component, OnInit, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { StoryModelWithContents } from '@stories/models/domain/story-model';
import { CommonModule } from '@angular/common';
import { take } from 'rxjs';
import { convertToBase64 } from '@shared/helpers/image.helper';
import { IteratorService } from '@shared/services/statefull/iterator.service';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { UserService } from '@users/services/user.service';

@Component({
  selector: 'app-read-story-content',
  standalone: true,
  imports: [CommonModule, ButtonModule, ProgressSpinnerModule],
  providers: [IteratorService],
  templateUrl: './read-story-content.component.html',
  styleUrl: './read-story-content.component.scss',
})
export class ReadStoryContentComponent implements OnInit {
  storyId: string | null = null;

  globalError: string | null = null;
  story: StoryModelWithContents | null = null;
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

          this.story = {
            ...story,
            ImagePreviewUrl: convertToBase64(story.ImagePreviewUrl),
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

  get base64Image(): string | undefined {
    return this.story?.ImagePreviewUrl;
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
