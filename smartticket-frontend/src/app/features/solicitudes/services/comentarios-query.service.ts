import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Comentario } from '../../../core/models/solicitud.models';
 
@Injectable({ providedIn: 'root' })
export class ComentariosQueryService {
  private readonly http = inject(HttpClient);
 
  listar(solicitudId: number): Observable<Comentario[]> {
    return this.http.get<Comentario[]>(environment.apiUrl + '/solicitudes/' + solicitudId + '/comentarios');
  }
}