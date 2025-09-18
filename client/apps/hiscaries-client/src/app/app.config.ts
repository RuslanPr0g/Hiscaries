import {
  ApplicationConfig,
  forwardRef,
  importProvidersFrom,
  InjectionToken,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter, withRouterConfig } from '@angular/router';
import { routes } from './app.routes';
import { provideState, provideStore } from '@ngrx/store';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { JwtModule } from '@auth0/angular-jwt';
import { environment } from '@environments/environment';
import { storyFeatureKey, storyReducer } from './stories/store/story.reducer';
import { NotificationHandler } from '@shared/models/notification-handler.model';
import { StoryPublishedHandler } from './users/notification-handlers/story-published-notification.handler';

export function tokenGetter() {
  return localStorage.getItem(environment.localStorageKeys.ACCESS_TOKEN_KEY);
}

export const NOTIFICATION_HANDLERS = new InjectionToken<NotificationHandler[]>(
  'NOTIFICATION_HANDLERS',
);

export const NotificationHandlerProviders = [
  {
    provide: NOTIFICATION_HANDLERS,
    useExisting: forwardRef(() => StoryPublishedHandler),
  },
];

export const appConfig: ApplicationConfig = {
  providers: [
    ...NotificationHandlerProviders,
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(
      routes,
      withRouterConfig({
        paramsInheritanceStrategy: 'always',
        onSameUrlNavigation: 'reload',
      }),
    ),
    provideStore(),
    importProvidersFrom(
      JwtModule.forRoot({
        config: {
          tokenGetter: tokenGetter,
          // TODO: make this thing come from CD/CI
          allowedDomains: ['localhost'],
          disallowedRoutes: ['/auth/login', '/auth/register'],
        },
      }),
    ),
    provideHttpClient(withInterceptorsFromDi()),
    provideState({ name: storyFeatureKey, reducer: storyReducer }),
    provideAnimationsAsync(),
  ],
};
