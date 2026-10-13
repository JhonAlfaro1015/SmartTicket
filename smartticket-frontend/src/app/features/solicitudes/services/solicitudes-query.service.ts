import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SolicitudDetalle, EventoHistorial } from '../../../core/models/solicitud.models';
 
@Injectable({ providedIn: 'root' })
export class SolicitudesQueryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl + '/solicitudes';
 
  // El servidor decide el alcance según el rol (el Agente solo recibe las suyas)
  listarTodas(): Observable<SolicitudDetalle[]> {
    return this.http.get<SolicitudDetalle[]>(this.baseUrl);
  }
 
  listarMias(): Observable<SolicitudDetalle[]> {
    return this.http.get<SolicitudDetalle[]>(this.baseUrl + '/mias');
  }
 
  obtenerPorId(id: number): Observable<SolicitudDetalle> {
    return this.http.get<SolicitudDetalle>(this.baseUrl + '/' + id);
  }
 
  obtenerHistorial(id: number): Observable<EventoHistorial[]> {
    return this.http.get<EventoHistorial[]>(this.baseUrl + '/' + id + '/historial');
  }
}