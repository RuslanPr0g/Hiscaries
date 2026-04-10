export interface ReadingSettings {
  DarkMode: boolean;
  PreferPdf: boolean;
}

export const defaultReadingSettings: ReadingSettings = {
  DarkMode: true,
  PreferPdf: false,
};
