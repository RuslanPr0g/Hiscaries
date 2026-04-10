import { Injectable, inject } from '@angular/core';
import { environment } from '@environments/environment';
import { DocumentContent } from '../models/document-content.model';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { OperationResult } from '@shared/models/operation-result.model';

@Injectable({
  providedIn: 'root',
})
export class MediaService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/media`;

  asContents(file: File, start?: number, end?: number): Observable<DocumentContent> {
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

  upload(storyId: string, file: File): Observable<OperationResult> {
    let url = `${this.apiUrl}/documents/upload`;
    const params = new HttpParams().set('storyId', storyId);

    return this.http.post<OperationResult>(url, file, {
      // TODO: for now only pdf
      headers: { 'Content-Type': 'application/pdf' },
      params,
    });
  }

  delete(storyId: string): Observable<OperationResult> {
    let url = `${this.apiUrl}/documents`;
    const params = new HttpParams().set('storyId', storyId);

    return this.http.delete<OperationResult>(url, { params });
  }
}
