import { Injectable, inject } from '@angular/core';
import { LocalStorageService } from '@shared/services/local-storage.service';
import {
  defaultReadingSettings,
  ReadingSettings,
} from '@stories/models/domain/reading-settings.model';

@Injectable({
  providedIn: 'root',
})
export class LoadReadingSettingsService {
  private readonly STORAGE_KEY = 'readingSettings';

  localStorage = inject(LocalStorageService);

  getSettings(): ReadingSettings {
    const value = this.localStorage.get<ReadingSettings>(this.STORAGE_KEY);

    if (!value) {
      this.localStorage.set<ReadingSettings>(this.STORAGE_KEY, defaultReadingSettings);
      return defaultReadingSettings;
    }

    return value;
  }

  setSettings(settings: ReadingSettings): void {
    this.localStorage.set<ReadingSettings>(this.STORAGE_KEY, settings);
  }
}
