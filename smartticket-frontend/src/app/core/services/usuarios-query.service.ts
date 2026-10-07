import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
 
export interface UsuarioBasico {
  usuarioId: number;
  nombre: string;
  email: string;
}
 
@Injectable({ providedIn: 'root' })
export class UsuariosQueryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/usuarios`;
 
  listarPorRol(rol: string): Observable<UsuarioBasico[]> {
    const params = new HttpParams().set('rol', rol);
    return this.http.get<UsuarioBasico[]>(this.baseUrl, { params });
  }
}