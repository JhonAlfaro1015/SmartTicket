import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
 
export interface CategoriaDto { categoriaId: number; nombre: string; activo: boolean; }
export interface PrioridadDto { prioridadId: number; nombre: string; nivelUrgencia: number; }
export interface AreaDto { areaId: number; nombre: string; activo: boolean; }
export interface EstadoDto { estadoId: number; nombre: string; esFinal: boolean; }
 
@Injectable({ providedIn: 'root' })
export class CatalogosQueryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/catalogos`;
  private cargado = false;
 
  readonly categorias = signal<CategoriaDto[]>([]);
  readonly prioridades = signal<PrioridadDto[]>([]);
  readonly areas = signal<AreaDto[]>([]);
  readonly estados = signal<EstadoDto[]>([]);
 
  cargarCatalogos(): void {
    if (this.cargado) return;
    this.cargado = true;
 
    this.http.get<CategoriaDto[]>(`${this.baseUrl}/categorias`).subscribe(r => this.categorias.set(r));
    this.http.get<PrioridadDto[]>(`${this.baseUrl}/prioridades`).subscribe(r => this.prioridades.set(r));
    this.http.get<AreaDto[]>(`${this.baseUrl}/areas`).subscribe(r => this.areas.set(r));
    this.http.get<EstadoDto[]>(`${this.baseUrl}/estados`).subscribe(r => this.estados.set(r));
  }
 
  nombreCategoria(id: number): string {
    return this.categorias().find(c => c.categoriaId === id)?.nombre ?? `#${id}`;
  }
 
  nombrePrioridad(id: number): string {
    return this.prioridades().find(p => p.prioridadId === id)?.nombre ?? `#${id}`;
  }
 
  nombreArea(id: number): string {
    return this.areas().find(a => a.areaId === id)?.nombre ?? `#${id}`;
  }
 
  nombreEstado(id: number): string {
    return this.estados().find(e => e.estadoId === id)?.nombre ?? `#${id}`;
  }
}