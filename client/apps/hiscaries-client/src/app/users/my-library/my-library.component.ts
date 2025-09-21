import { Component, inject, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { LibraryComponent } from '@users/library/library.component';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { AuthService } from '@users/services/auth.service';
import { UserService } from '@users/services/user.service';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { NavigationConst } from '@shared/constants/navigation.const';
import { LibraryModel } from '@users/models/domain/library.model';
import { CommonModule } from '@angular/common';
import { generateEmptyQueriedResult, QueriedModel } from '@shared/models/queried.model';
import { StoryModel } from '@stories/models/domain/story-model';
import { ModifyLibraryModel } from '@users/models/domain/modify-library.model';

@Component({
  selector: 'app-my-library',
  standalone: true,
  imports: [LibraryComponent, CommonModule],
  templateUrl: './my-library.component.html',
  styleUrls: ['./my-library.component.scss'],
  providers: [PaginationService],
})
export class MyLibraryComponent implements AfterViewInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private storyService = inject(StoryWithMetadataService);
  pagination = inject(PaginationService);

  libraryInfo: LibraryModel | null = null;
  stories = signal<QueriedModel<StoryModel>>(generateEmptyQueriedResult());
  isLoading = signal(false);

  @ViewChild('loadMoreAnchor', { static: true }) loadMoreAnchor!: ElementRef<HTMLDivElement>;
  private observer!: IntersectionObserver;

  constructor() {
    if (!this.authService.isPublisher()) {
      this.router.navigate([NavigationConst.Home]);
      return;
    }
    this.fetchLibrary();
  }

  ngAfterViewInit() {
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting && !this.isLoading()) {
            this.nextPage();
          }
        });
      },
      { threshold: 0 },
    );

    if (this.loadMoreAnchor) this.observer.observe(this.loadMoreAnchor.nativeElement);
  }

  private fetchLibrary() {
    this.isLoading.set(true);
    this.userService
      .getLibrary()
      .pipe(take(1))
      .subscribe({
        next: (library) => this.processLibrary(library),
        error: () => this.router.navigate([NavigationConst.Home]),
      });
  }

  private processLibrary(library: LibraryModel) {
    if (!library) return this.router.navigate([NavigationConst.Home]);
    if (
      !library.IsLibraryOwner ||
      !this.authService.isTokenOwnerByUsername(library.PlatformUser.Username)
    ) {
      return this.router.navigate([NavigationConst.PublisherLibrary(library.Id)]);
    }
    this.libraryInfo = library;
    this.pagination.reset();
    this.loadStories(true);
    return null;
  }

  private loadStories(reset = false) {
    if (!this.libraryInfo) return;
    if (reset) {
      this.pagination.reset();
      this.stories.set(generateEmptyQueriedResult());
    }

    this.isLoading.set(true);
    this.storyService
      .getStoriesByLibrary({
        LibraryId: this.libraryInfo.Id,
        QueryableModel: this.pagination.snapshot,
      })
      .pipe(take(1))
      .subscribe({
        next: (data) => {
          const current = reset ? generateEmptyQueriedResult<StoryModel>() : this.stories();
          this.stories.set({
            Items: [...current.Items, ...data.Items],
            TotalItemsCount: data.TotalItemsCount,
          });
        },
        complete: () => this.isLoading.set(false),
        error: () => this.isLoading.set(false),
      });
  }

  editLibrary(model: ModifyLibraryModel) {
    const shouldUpdateAvatar = !model.AvatarUrl || model.AvatarUrl.startsWith('data');
    this.userService
      .editLibrary({
        LibraryId: model.Id,
        Bio: model.Bio,
        Avatar: shouldUpdateAvatar ? model.AvatarUrl : null,
        LinksToSocialMedia: model.LinksToSocialMedia,
        ShouldUpdateAvatar: shouldUpdateAvatar,
      })
      .pipe(take(1))
      .subscribe(() => this.fetchLibrary());
  }

  nextPage() {
    this.pagination.nextPage();
    this.loadStories();
  }

  prevPage() {
    if (this.pagination.snapshot.StartIndex === 0) return;
    this.pagination.prevPage();
    this.loadStories();
  }
}
