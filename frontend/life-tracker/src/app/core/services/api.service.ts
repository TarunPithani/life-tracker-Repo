import { HttpClient, HttpContext, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface ApiRequestOptions {
  headers?: HttpHeaders | Record<string, string | string[]>;
  context?: HttpContext;
  params?:
    | HttpParams
    | Record<
        string,
        string | number | boolean | ReadonlyArray<string | number | boolean>
      >;
  reportProgress?: boolean;
  withCredentials?: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl.replace(/\/+$/, '');

  get<T>(endpoint: string, options: ApiRequestOptions = {}): Observable<T> {
    return this.http.get<T>(this.buildUrl(endpoint), options);
  }

  post<T>(
    endpoint: string,
    body: unknown,
    options: ApiRequestOptions = {},
  ): Observable<T> {
    return this.http.post<T>(this.buildUrl(endpoint), body, options);
  }

  put<T>(
    endpoint: string,
    body: unknown,
    options: ApiRequestOptions = {},
  ): Observable<T> {
    return this.http.put<T>(this.buildUrl(endpoint), body, options);
  }

  patch<T>(
    endpoint: string,
    body: unknown,
    options: ApiRequestOptions = {},
  ): Observable<T> {
    return this.http.patch<T>(this.buildUrl(endpoint), body, options);
  }

  delete<T>(endpoint: string, options: ApiRequestOptions = {}): Observable<T> {
    return this.http.delete<T>(this.buildUrl(endpoint), options);
  }

  private buildUrl(endpoint: string): string {
    if (/^https?:\/\//i.test(endpoint)) {
      return endpoint;
    }

    return `${this.baseUrl}/${endpoint.replace(/^\/+/, '')}`;
  }
}
