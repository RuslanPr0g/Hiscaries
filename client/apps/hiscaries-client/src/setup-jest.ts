import { setupZoneTestEnv } from 'jest-preset-angular/setup-env/zone';

setupZoneTestEnv();

Object.defineProperty(window, 'TextEncoder', {
  writable: true,
});
