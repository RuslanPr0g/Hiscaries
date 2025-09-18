import { Component, inject, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { LibraryComponent } from '@users/library/library.component';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { NavigationConst } from '@shared/constants/navigation.const';
import { UserService } from '@users/services/user.service';
import { StoryWithMetadataService } from '@user-to-story/services/multiple-services-merged/story-with-metadata.service';
import { PaginationService } from '@shared/services/statefull/pagination.service';
import { LibraryModel } from '@users/models/domain/library.model';
import { CommonModule } from '@angular/common';
import { emptyQueriedResult, QueriedModel } from '@shared/models/queried.model';
import { StoryModel } from '@stories/models/domain/story-model';

@Component({
  selector: 'app-publisher-library',
  standalone: true,
  imports: [LibraryComponent, CommonModule],
  templateUrl: './publisher-library.component.html',
  styleUrls: ['./publisher-library.component.scss'],
  providers: [PaginationService],
})
export class PublisherLibraryComponent implements AfterViewInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private userService = inject(UserService);
  private storyService = inject(StoryWithMetadataService);
  pagination = inject(PaginationService);

  libraryId = this.route.snapshot.paramMap.get('id');
  libraryInfo: LibraryModel | null = null;
  stories = signal<QueriedModel<StoryModel>>(emptyQueriedResult);
  isLoading = signal(false);
  isSubscribeLoading = false;

  @ViewChild('loadMoreAnchor', { static: true }) loadMoreAnchor!: ElementRef<HTMLDivElement>;
  private observer!: IntersectionObserver;

  constructor() {
    if (!this.libraryId) this.router.navigate([NavigationConst.Home]);
    else this.fetchLibrary(this.libraryId);
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

  private fetchLibrary(libraryId: string | null) {
    if (!libraryId) return this.router.navigate([NavigationConst.Home]);
    this.isLoading.set(true);
    this.userService
      .getLibrary(libraryId)
      .pipe(take(1))
      .subscribe({
        next: (library) => this.processLibrary(library),
        error: () => this.router.navigate([NavigationConst.Home]),
      });
    return null;
  }

  private processLibrary(library: LibraryModel) {
    if (!library) return this.router.navigate([NavigationConst.Home]);
    if (library.IsLibraryOwner) return this.router.navigate([NavigationConst.MyLibrary]);
    this.libraryInfo = library;
    this.pagination.reset();
    this.loadStories(true);
    return null;
  }

  private loadStories(reset: boolean = false) {
    if (!this.libraryInfo) return;
    if (reset) {
      this.pagination.reset();
      this.stories.set(emptyQueriedResult);
    }
    this.isLoading.set(true);
    this.storyService
      .getStoriesByLibrary({ LibraryId: this.libraryId!, QueryableModel: this.pagination.snapshot })
      .pipe(take(1))
      .subscribe({
        next: (data) => {
          const current = reset ? emptyQueriedResult : this.stories();
          this.stories.set({
            Items: [...current.Items, ...data.Items],
            TotalItemsCount: data.TotalItemsCount,
          });
        },
        complete: () => this.isLoading.set(false),
        error: () => this.isLoading.set(false),
      });
  }

  get isSubscribed() {
    return this.libraryInfo?.IsSubscribed ?? false;
  }

  subscribeAction() {
    this.updateSubscription(true);
  }

  unSubscribeAction() {
    this.updateSubscription(false);
  }

  private updateSubscription(subscribe: boolean) {
    if (!this.libraryId) return;
    this.isSubscribeLoading = true;
    const action = subscribe
      ? this.userService.subscribeToLibrary({ LibraryId: this.libraryId })
      : this.userService.unsubscribeFromLibrary({ LibraryId: this.libraryId });
    action.pipe(take(1)).subscribe({
      next: () => {
        const newCount = (this.libraryInfo?.SubscribersCount ?? 0) + (subscribe ? 1 : -1);
        this.fetchLibrary(this.libraryId);
        setTimeout(() => {
          if (this.libraryInfo) this.libraryInfo.SubscribersCount = newCount;
          this.isSubscribeLoading = false;
        }, 1000);
      },
      error: () => (this.isSubscribeLoading = false),
    });
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
