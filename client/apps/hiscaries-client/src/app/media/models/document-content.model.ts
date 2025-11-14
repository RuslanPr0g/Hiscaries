export interface DocumentContent {
  Pages: DocumentPage[];
}

export interface DocumentPage {
  Page: number;
  Text: string;
}
