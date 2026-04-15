import { DocumentContent } from '../models/document-content.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '@environments/environment';
import { OperationResult } from '@shared/models/operation-result.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MediaService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/media`;

  asContents(file: File, start?: number, end?: number): Observable<DocumentContent> {
    const params = new URLSearchParams();
    if (start != null) params.append('start', start.toString());
    if (end != null) params.append('end', end.toString());
    const url = params.toString()
      ? `${this.apiUrl}/documents/as-contents?${params.toString()}`
      : `${this.apiUrl}/documents/as-contents`;

    return this.http.post<DocumentContent>(url, file, {
      // TODO: for now only pdf
      headers: { 'Content-Type': 'application/pdf' },
    });
  }

  upload(storyId: string, file: File): Observable<OperationResult> {
    const url = `${this.apiUrl}/documents/upload`;
    const params = new HttpParams().set('storyId', storyId);

    return this.http.post<OperationResult>(url, file, {
      // TODO: for now only pdf
      headers: { 'Content-Type': 'application/pdf' },
      params,
    });
  }

  delete(storyId: string): Observable<OperationResult> {
    const url = `${this.apiUrl}/documents`;
    const params = new HttpParams().set('storyId', storyId);

    return this.http.delete<OperationResult>(url, { params });
  }

  uploadUserAnnotatedPdf(storyId: string, blob: Blob): Observable<OperationResult> {
    const url = `${this.apiUrl}/user-documents?storyId=${storyId}`;

    return this.http.post<OperationResult>(url, blob, {
      headers: { 'Content-Type': 'application/pdf' },
    });
  }

  deleteUserAnnotatedPdf(storyId: string): Observable<OperationResult> {
    const url = `${this.apiUrl}/user-documents/${storyId}`;

    return this.http.delete<OperationResult>(url);
  }
}
