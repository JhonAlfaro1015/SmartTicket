import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SolicitudResponse } from '../../../core/models/solicitud.models';
 
@Injectable({ providedIn: 'root' })
export class SolicitudesQueryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/solicitudes`;
 
  listarMias(): Observable<SolicitudResponse[]> {
    return this.http.get<SolicitudResponse[]>(`${this.baseUrl}/mias`);
  }
 
  listarTodas(estadoId?: number, prioridadId?: number): Observable<SolicitudResponse[]> {
    let params = new HttpParams();
    if (estadoId) params = params.set('estadoId', estadoId);
    if (prioridadId) params = params.set('prioridadId', prioridadId);
    return this.http.get<SolicitudResponse[]>(this.baseUrl, { params });
  }
 
  obtenerPorId(id: number): Observable<SolicitudResponse> {
    return this.http.get<SolicitudResponse>(`${this.baseUrl}/${id}`);
  }
}