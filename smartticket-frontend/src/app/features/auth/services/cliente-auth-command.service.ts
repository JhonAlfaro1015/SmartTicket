import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthResponse, ClienteLoginRequest, ClienteRegisterRequest } from '../../../core/models/auth.models';
 
@Injectable({ providedIn: 'root' })
export class ClienteAuthCommandService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/cliente-auth`;
 
  login(request: ClienteLoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request);
  }
 
  registrar(request: ClienteRegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/registro`, request);
  }
}