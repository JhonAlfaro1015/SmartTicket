import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Comentario } from '../../../core/models/solicitud.models';
 
@Injectable({ providedIn: 'root' })
export class ComentariosCommandService {
  private readonly http = inject(HttpClient);
 
  publicar(solicitudId: number, texto: string): Observable<Comentario> {
    return this.http.post<Comentario>(
      environment.apiUrl + '/solicitudes/' + solicitudId + '/comentarios', { texto });
  }
}