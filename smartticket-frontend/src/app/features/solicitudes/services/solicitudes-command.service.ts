import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  AsignarSolicitudRequest, CambiarEstadoRequest, CreateSolicitudRequest, SolicitudResponse
} from '../../../core/models/solicitud.models';
 
@Injectable({ providedIn: 'root' })
export class SolicitudesCommandService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl + '/solicitudes';
 
  crear(request: CreateSolicitudRequest): Observable<SolicitudResponse> {
    return this.http.post<SolicitudResponse>(this.baseUrl, request);
  }
 
  cambiarEstado(solicitudId: number, request: CambiarEstadoRequest): Observable<void> {
    return this.http.put<void>(this.baseUrl + '/' + solicitudId + '/estado', request);
  }
 
  asignar(solicitudId: number, request: AsignarSolicitudRequest): Observable<void> {
    return this.http.put<void>(this.baseUrl + '/' + solicitudId + '/asignar', request);
  }
}