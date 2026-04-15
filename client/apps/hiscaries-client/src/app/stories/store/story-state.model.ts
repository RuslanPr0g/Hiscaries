export interface StoryStateModel {
  SearchTerm: string | null;
  PdfSaving: boolean;
  PdfSaveError: string | null;
}

export const InitialStoryStateModel: StoryStateModel = {
  SearchTerm: null,
  PdfSaving: false,
  PdfSaveError: null,
};
