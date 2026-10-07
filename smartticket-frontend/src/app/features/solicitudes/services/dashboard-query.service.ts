import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { DashboardResumen } from '../../../core/models/dashboard.models';
 
@Injectable({ providedIn: 'root' })
export class DashboardQueryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/dashboard`;
 
  obtenerResumen(): Observable<DashboardResumen> {
    return this.http.get<DashboardResumen>(`${this.baseUrl}/resumen`);
  }
}