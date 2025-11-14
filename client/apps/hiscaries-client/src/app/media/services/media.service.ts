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

  uploadPdf(file: File, start?: number, end?: number): Observable<DocumentContent> {
    let url = `${this.apiUrl}/documents/as-contents`;
    const params = new URLSearchParams();
    if (start != null) params.append('start', start.toString());
    if (end != null) params.append('end', end.toString());
    if (params.toString()) url += `?${params.toString()}`;

    return this.http.post<DocumentContent>(url, file, {
      // TODO: for now only pdf
      headers: { 'Content-Type': 'application/pdf' },
    });
  }
}
