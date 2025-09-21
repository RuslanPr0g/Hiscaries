import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StoryModel } from '@stories/models/domain/story-model';
import { take } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ButtonTwoComponent } from '@shared/components/button-two/button-two.component';
import { NavigationConst } from '@shared/constants/navigation.const';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { defaultQueryableModel } from '@shared/models/queryable.model';

@Component({
  selector: 'app-preview-story',
  standalone: true,
  imports: [CommonModule, ButtonTwoComponent],
  templateUrl: './preview-story.component.html',
  styleUrl: './preview-story.component.scss',
})
export class PreviewStoryComponent implements OnInit, OnDestroy {
  private storyId: string | null = null;

  story: StoryModel | null = null;
  storyNotFound = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private storyService: StoryWithMetadataService,
  ) {}

  ngOnInit(): void {
    this.storyId = this.route.snapshot.paramMap.get('id');

    this.storyService
      .searchStory({
        Id: this.storyId,
        QueryableModel: defaultQueryableModel,
      })
      .pipe(take(1))
      .subscribe((stories) => {
        const story = stories.Items[0];
        if (!story) {
          this.storyNotFound = true;
        } else {
          this.story = story;
          this.setBackgroundImage();
        }
      });
  }

  ngOnDestroy(): void {
    document.body.style.backgroundImage = '';
    document.body.classList.remove('story-background-overlay');
  }

  get backgroundImageUrl(): string | undefined {
    return (
      this.story?.ImagePreviewUrl?.Large ??
      this.story?.ImagePreviewUrl?.Medium ??
      this.story?.ImagePreviewUrl?.Small
    );
  }

  get isEditable(): boolean {
    return this.story?.IsEditable ?? false;
  }

  readStory(): void {
    this.router.navigate([NavigationConst.ReadStory(this.storyId!)]);
  }

  modifyStory(): void {
    this.router.navigate([NavigationConst.ModifyStory(this.storyId!)]);
  }

  navigateToLibrary(): void {
    if (this.story?.LibraryId) {
      this.router.navigate([NavigationConst.PublisherLibrary(this.story?.LibraryId)]);
    } else {
      console.warn('Why the story library id is empty?');
    }
  }

  private setBackgroundImage() {
    if (this.backgroundImageUrl) {
      document.body.style.backgroundImage = `url(${this.backgroundImageUrl})`;
      document.body.style.backgroundSize = 'cover';
      document.body.style.backgroundPosition = 'center';
      document.body.style.backgroundRepeat = 'no-repeat';
      document.body.style.backgroundAttachment = 'fixed';
      document.body.classList.add('story-background-overlay');
    }
  }
}
