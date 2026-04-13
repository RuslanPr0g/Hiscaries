import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '@environments/environment';
import { LoginUserRequest } from '@users/models/requests/login-user.model';
import { RegisterUserRequest } from '@users/models/requests/register-user.model';
import { UserWithTokenResponse } from '@users/models/response/user-with-token.model';
import { Observable, Subject, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private jwtHelper = inject(JwtHelperService);

  private apiUrl = `${environment.apiUrl}/accounts`;
  private access_token_local_storage_key: string = environment.localStorageKeys.ACCESS_TOKEN_KEY;
  private refresh_token_local_storage_key: string = environment.localStorageKeys.REFRESH_TOKEN_KEY;

  private loginSubject = new Subject<void>();
  private logoutSubject = new Subject<void>();

  loginEvent$ = this.loginSubject.asObservable();
  logoutEvent$ = this.logoutSubject.asObservable();

  register(request: RegisterUserRequest): Observable<UserWithTokenResponse> {
    return this.http.post<UserWithTokenResponse>(this.apiUrl + '/register', request).pipe(
      tap((tokenData: UserWithTokenResponse) => {
        localStorage.setItem(this.access_token_local_storage_key, tokenData.Token);
        localStorage.setItem(this.refresh_token_local_storage_key, tokenData.RefreshToken);
        this.loginSubject.next();
      }),
    );
  }

  login(request: LoginUserRequest): Observable<UserWithTokenResponse> {
    return this.http.post<UserWithTokenResponse>(this.apiUrl + '/login', request).pipe(
      tap((tokenData: UserWithTokenResponse) => {
        localStorage.setItem(this.access_token_local_storage_key, tokenData.Token);
        localStorage.setItem(this.refresh_token_local_storage_key, tokenData.RefreshToken);
        this.loginSubject.next();
      }),
    );
  }

  logOut(): void {
    localStorage.removeItem(this.access_token_local_storage_key);
    localStorage.removeItem(this.refresh_token_local_storage_key);
    this.logoutSubject.next();
  }

  getToken(): string | null {
    return localStorage.getItem(this.access_token_local_storage_key);
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem(this.access_token_local_storage_key);
    const authenticated = token != null && !this.jwtHelper.isTokenExpired(token);
    if (!authenticated) {
      this.logOut();
      return false;
    }

    return true;
  }

  isTokenOwner(userId?: string): boolean {
    const token = localStorage.getItem(this.access_token_local_storage_key);

    if (!token) {
      return false;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);

    const id = decodedToken?.id;

    return id === userId;
  }

  isTokenOwnerByUsername(username?: string): boolean {
    const token = localStorage.getItem(this.access_token_local_storage_key);

    if (!token) {
      return false;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);

    const usernameFromToken = decodedToken?.username;

    return username === usernameFromToken;
  }

  isPublisher(): boolean {
    const token = localStorage.getItem(this.access_token_local_storage_key);

    if (!token) {
      return false;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);

    const role = decodedToken?.role;

    return role === 'publisher';
  }

  isAdmin(): boolean {
    const token = localStorage.getItem(this.access_token_local_storage_key);

    if (!token) {
      return false;
    }

    if (this.jwtHelper.isTokenExpired(token)) {
      return false;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);

    const role = decodedToken?.role;

    return role === 'admin';
  }

  getUserRole(): string | null {
    const token = localStorage.getItem(this.access_token_local_storage_key);

    if (!token) {
      return null;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);

    return decodedToken?.role ?? null;
  }

  getUserId(): string | null {
    const token = localStorage.getItem(this.access_token_local_storage_key);

    if (!token) {
      return null;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);

    return decodedToken?.id ?? null;
  }
}
