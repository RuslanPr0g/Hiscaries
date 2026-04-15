import { routes } from './app.routes';
import { StoryPdfEffects } from './stories/store/story-pdf.effects';
import { storyFeatureKey, storyReducer } from './stories/store/story.reducer';
import { StoryPublishedHandler } from './users/notification-handlers/story-published-notification.handler';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import {
  ApplicationConfig,
  forwardRef,
  importProvidersFrom,
  InjectionToken,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter, withRouterConfig } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { environment } from '@environments/environment';
import { provideEffects } from '@ngrx/effects';
import { provideState, provideStore } from '@ngrx/store';
import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';
import { NotificationHandler } from '@shared/models/notification-handler.model';
import { MessageService } from 'primeng/api';
import { providePrimeNG } from 'primeng/config';

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
    MessageService,
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
    provideAnimations(),
    provideState({ name: storyFeatureKey, reducer: storyReducer }),
    provideEffects(StoryPdfEffects),
    providePrimeNG({
      theme: {
        preset: definePreset(Aura, {
          semantic: {
            primary: {
              50: '#f9f3ed',
              100: '#f0e3d0',
              200: '#e3c9a8',
              300: '#d4aa7d',
              400: '#c9a27e',
              500: '#c9a27e',
              600: '#a37c5b',
              700: '#8a6347',
              800: '#6e4e38',
              900: '#523a2a',
              950: '#3a2820',
            },
            colorScheme: {
              light: {
                surface: {
                  0: 'rgba(255,255,255,0.08)',
                  50: 'rgba(255,255,255,0.06)',
                  100: 'rgba(255,255,255,0.08)',
                  200: 'rgba(255,255,255,0.10)',
                  300: 'rgba(255,255,255,0.15)',
                  400: 'rgba(255,255,255,0.25)',
                  500: 'rgba(255,255,255,0.40)',
                  600: 'rgba(255,255,255,0.55)',
                  700: '#f5f0eb',
                  800: '#f5f0eb',
                  900: '#f5f0eb',
                  950: '#f5f0eb',
                },
                formField: {
                  background: 'rgba(255,255,255,0.08)',
                  disabledBackground: 'rgba(255,255,255,0.04)',
                  filledBackground: 'rgba(255,255,255,0.10)',
                  borderColor: 'rgba(255,255,255,0.15)',
                  hoverBorderColor: '#d1bfa3',
                  focusBorderColor: '#c9a27e',
                  color: '#f5f0eb',
                  placeholderColor: 'rgba(255,255,255,0.45)',
                  iconColor: '#d1bfa3',
                  shadow: 'none',
                },
                text: {
                  color: '#f5f0eb',
                  hoverColor: '#ffffff',
                  mutedColor: 'rgba(245,240,235,0.65)',
                  hoverMutedColor: 'rgba(245,240,235,0.85)',
                },
                content: {
                  background: 'rgba(56,41,29,0.4)',
                  hoverBackground: 'rgba(255,255,255,0.06)',
                  borderColor: 'rgba(255,255,255,0.10)',
                  color: '#f5f0eb',
                  hoverColor: '#ffffff',
                },
                overlay: {
                  select: {
                    background: '#3a2e2a',
                    borderColor: 'rgba(255,255,255,0.12)',
                    color: '#f5f0eb',
                  },
                  popover: {
                    background: '#3a2e2a',
                    borderColor: 'rgba(255,255,255,0.12)',
                    color: '#f5f0eb',
                  },
                  modal: {
                    background: 'rgba(56,41,29,0.85)',
                    borderColor: 'rgba(255,255,255,0.12)',
                    color: '#f5f0eb',
                  },
                },
                list: {
                  option: {
                    focusBackground: 'rgba(255,255,255,0.08)',
                    selectedBackground: '#c9a27e',
                    selectedFocusBackground: '#a37c5b',
                    color: '#f5f0eb',
                    focusColor: '#ffffff',
                    selectedColor: '#1e1e1e',
                    selectedFocusColor: '#1e1e1e',
                  },
                  optionGroup: { background: 'transparent', color: 'rgba(245,240,235,0.65)' },
                },
                highlight: {
                  background: '#c9a27e',
                  focusBackground: '#a37c5b',
                  color: '#1e1e1e',
                  focusColor: '#1e1e1e',
                },
              },
            },
          },
        }),
        options: {
          darkModeSelector: false,
          cssLayer: {
            name: 'primeng',
            order: 'tailwind-base, primeng, tailwind-utilities',
          },
        },
      },
    }),
  ],
};
