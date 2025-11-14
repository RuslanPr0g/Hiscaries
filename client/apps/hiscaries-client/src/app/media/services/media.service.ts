import { Injectable } from '@angular/core';
import { environment } from '@environments/environment';
import { DocumentContent } from '../models/document-content.model';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class MediaService {
  private apiUrl = `${environment.apiUrl}/media`;

  constructor(private http: HttpClient) {}

  uploadPdf(file: File): Observable<DocumentContent> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<DocumentContent>(`${this.apiUrl}/document/as-contents`, formData);
  }
}
