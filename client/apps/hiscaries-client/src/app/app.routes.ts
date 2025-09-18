import { Routes } from '@angular/router';
import { LoginComponent } from './users/login/login.component';
import { HomeComponent } from './users/home/home.component';
import { authGuard } from '@shared/auth/guards/auth.guard';
import { provideState } from '@ngrx/store';
import { PublishStoryComponent } from './stories/publish-story/publish-story.component';
import { PreviewStoryComponent } from './stories/preview-story/preview-story.component';
import { ModifyStoryComponent } from './stories/modify-story/modify-story.component';
import { ReadStoryContentComponent } from './stories/read-story-content/read-story-content.component';
import { SearchStoryComponent } from './stories/search-story/search-story.component';
import { storyFeatureKey, storyReducer } from './stories/store/story.reducer';
import { ReadingHistoryComponent } from './stories/reading-history/reading-history.component';
import { BecomePublisherComponent } from './users/become-publisher/become-publisher.component';
import { MyLibraryComponent } from './users/my-library/my-library.component';
import { PublisherLibraryComponent } from './users/publisher-library/publisher-library.component';

export const routes: Routes = [
  {
    path: 'login',
    title: 'Login',
    component: LoginComponent,
  },
  {
    path: '',
    title: 'Home page',
    component: HomeComponent,
    canActivate: [authGuard],
  },
  {
    path: 'become-publisher',
    title: 'Become Publisher',
    component: BecomePublisherComponent,
    canActivate: [authGuard],
  },
  {
    path: 'publish-story',
    title: 'Publish Story',
    component: PublishStoryComponent,
    canActivate: [authGuard],
  },
  {
    path: 'reading-history',
    title: 'Reading History',
    component: ReadingHistoryComponent,
    canActivate: [authGuard],
  },
  {
    path: 'library',
    title: 'My Library',
    component: MyLibraryComponent,
    canActivate: [authGuard],
  },
  {
    path: 'library/:id',
    title: 'Library',
    component: PublisherLibraryComponent,
    canActivate: [authGuard],
  },
  {
    path: 'modify-story/:id',
    title: 'Modify Story',
    component: ModifyStoryComponent,
    canActivate: [authGuard],
  },
  {
    path: 'preview-story/:id',
    title: 'Preview Story',
    component: PreviewStoryComponent,
    canActivate: [authGuard],
  },
  {
    path: 'read-story/:id',
    title: 'Read Story',
    component: ReadStoryContentComponent,
    canActivate: [authGuard],
  },
  {
    path: 'search-story/:term',
    title: 'Search Story',
    component: SearchStoryComponent,
    canActivate: [authGuard],
    providers: [provideState({ name: storyFeatureKey, reducer: storyReducer })],
  },
  { path: '**', redirectTo: '', pathMatch: 'full' },
];
