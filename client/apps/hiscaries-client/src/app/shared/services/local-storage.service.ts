import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LocalStorageService {
  /**
   * Retrieve a typed value from localStorage.
   * Automatically parses JSON if possible.
   */
  get<T = unknown>(key: string, defaultValue: T | null = null): T | null {
    try {
      const raw = localStorage.getItem(key);
      if (raw === null) return defaultValue;

      try {
        return JSON.parse(raw) as T;
      } catch {
        return raw as unknown as T;
      }
    } catch {
      return defaultValue;
    }
  }

  /**
   * Save a typed value to localStorage.
   * Automatically stringifies objects.
   */
  set<T>(key: string, value: T): void {
    try {
      const serialized = typeof value === 'string' ? value : JSON.stringify(value);

      localStorage.setItem(key, serialized);
    } catch {
      console.warn(`LocalStorageService: Unable to save key "${key}".`);
    }
  }

  /**
   * Remove a key from localStorage.
   */
  remove(key: string): void {
    try {
      localStorage.removeItem(key);
    } catch {
      console.warn(`LocalStorageService: Unable to remove key "${key}".`);
    }
  }

  /**
   * Clear all keys from localStorage.
   */
  clear(): void {
    try {
      localStorage.clear();
    } catch {
      console.warn(`LocalStorageService: Unable to clear localStorage.`);
    }
  }
}
