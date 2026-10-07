import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ClienteResponse } from '../../../core/models/cliente.models';
 
@Injectable({ providedIn: 'root' })
export class ClientesQueryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/clientes`;
 
  obtenerPorId(id: number): Observable<ClienteResponse> {
    return this.http.get<ClienteResponse>(`${this.baseUrl}/${id}`);
  }
}