import { Component, computed, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { visualPrioridad } from '../../../core/constants/visual';
import { SolicitudDetalle } from '../../../core/models/solicitud.models';
import { EstadoDto } from '../../../core/services/catalogos-query.service';
import { UsuarioBasico } from '../../../core/services/usuarios-query.service';
import { tiempoRelativo } from '../../../core/utils/fechas';
import { iniciales } from '../../../core/utils/texto';
import { AsignacionAgente } from '../../../shared/components/asignacion-agente';
import { EstadoBadge } from '../../../shared/components/estado-badge';
import { EstadoStepper } from '../../../shared/components/estado-stepper';
import { PrioridadBadge } from '../../../shared/components/prioridad-badge';
import { ScoreMeter } from '../../../shared/components/score-meter';
import { SlaChip } from '../../../shared/components/sla-chip';
 
@Component({
  selector: 'app-solicitud-card',
  standalone: true,
  imports: [
    MatButtonModule, MatIconModule, AsignacionAgente, EstadoBadge, EstadoStepper,
    PrioridadBadge, ScoreMeter, SlaChip
  ],
  template: `
    <article class="card" [class.card--final]="s().estadoEsFinal">
      <span class="barra" [class]="prioridadClase()"></span>
 
      <div class="cuerpo">
        <header class="cabecera" role="button" tabindex="0" (click)="abrir.emit()" (keydown.enter)="abrir.emit()">
          <div class="titulos">
            <div class="fila-id">
              <span class="id">#{{ s().solicitudId }}</span>
              @if (s().estadoEsFinal) {
                <span class="cinta"><mat-icon>lock</mat-icon>{{ s().estadoNombre }}</span>
              }
            </div>
            <h3>{{ s().categoriaNombre }}</h3>
            <p class="meta">
              Área: {{ s().areaNombre }}@if (s().subcategoriaNombre) { · {{ s().subcategoriaNombre }} }
            </p>
          </div>
 
          <div class="indicadores">
            <app-prioridad-badge [nombre]="s().prioridadNombre" [grande]="true" />
            <app-estado-badge [nombre]="s().estadoNombre" [grande]="true" />
            <app-score-meter [valor]="s().puntuacion" />
          </div>
        </header>
 
        <p class="texto">{{ s().textoOriginal }}</p>
 
        <div class="info">
          @if (modo() === 'staff') {
            <span class="dato"><mat-icon>person</mat-icon>{{ s().clienteNombre }}</span>
          }
          <span class="dato"><mat-icon>event</mat-icon>{{ hace() }}</span>
          @if (s().agenteNombre; as agente) {
            <span class="dato"><span class="avatar">{{ iniciales(agente) }}</span>{{ agente }}</span>
          } @else {
            <span class="dato sin-asignar"><mat-icon>person_off</mat-icon>Sin asignar</span>
          }
          <app-sla-chip [fechaLimite]="s().fechaLimiteSLA" [esFinal]="s().estadoEsFinal" />
        </div>
 
        <footer class="pie">
          <app-estado-stepper [estados]="estados()" [estadoActualId]="s().estadoId"
                              [siguientes]="s().estadosSiguientes"
                              (cambiar)="cambiarEstado.emit($event)" />
          <div class="acciones">
            @if (modo() === 'staff' && s().puedeAsignar) {
              <app-asignacion-agente [agentes]="agentes()" [agenteActualId]="s().usuarioAsignadoId"
                                     (asignar)="asignar.emit($event)" />
            }
            <button mat-stroked-button (click)="abrir.emit()">
              <mat-icon>forum</mat-icon>{{ modo() === 'staff' ? 'Detalle y comentarios' : 'Ver seguimiento' }}
            </button>
          </div>
        </footer>
      </div>
    </article>`,
  styles: [`
    :host { display: block; }
    .card { display: flex; background: #fff; border-radius: 14px; overflow: hidden;
            box-shadow: 0 1px 3px rgba(15,23,42,.08), 0 4px 14px rgba(15,23,42,.05);
            transition: box-shadow .15s, transform .15s; }
    .card:hover { box-shadow: 0 2px 6px rgba(15,23,42,.12), 0 8px 22px rgba(15,23,42,.08); transform: translateY(-1px); }
    .barra { flex: 0 0 6px; background: var(--pr-color); }
    .cuerpo { flex: 1; padding: 16px 20px 14px; min-width: 0; }
 
    .cabecera { display: flex; justify-content: space-between; gap: 16px; cursor: pointer; outline: none; }
    .cabecera:focus-visible { box-shadow: 0 0 0 2px #93c5fd; border-radius: 8px; }
    .fila-id { display: flex; align-items: center; gap: 8px; }
    .id { font-size: 12px; font-weight: 700; color: #64748b; letter-spacing: .3px; }
    h3 { margin: 2px 0 2px; font-size: 19px; font-weight: 700; color: #0f172a; line-height: 1.25; }
    .meta { margin: 0; font-size: 13px; color: #64748b; }
    .indicadores { display: flex; align-items: center; gap: 10px; flex-wrap: wrap; justify-content: flex-end; }
 
    .cinta { display: inline-flex; align-items: center; gap: 3px; font-size: 11px; font-weight: 700;
             text-transform: uppercase; letter-spacing: .4px; background: #475569; color: #fff;
             padding: 2px 8px 2px 4px; border-radius: 6px; }
    .cinta mat-icon { font-size: 14px; width: 14px; height: 14px; }
 
    .texto { margin: 12px 0; font-size: 14.5px; line-height: 1.45; color: #334155;
             display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
 
    .info { display: flex; align-items: center; flex-wrap: wrap; gap: 8px 18px; font-size: 13px; color: #475569; }
    .dato { display: inline-flex; align-items: center; gap: 5px; }
    .dato mat-icon { font-size: 17px; width: 17px; height: 17px; color: #94a3b8; }
    .avatar { display: inline-grid; place-items: center; width: 24px; height: 24px; border-radius: 50%;
              background: #1e3a5f; color: #fff; font-size: 10px; font-weight: 700; }
    .sin-asignar { color: #94a3b8; font-style: italic; }
 
    .pie { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;
           margin-top: 14px; padding-top: 12px; border-top: 1px solid #eef2f6; }
    .acciones { display: flex; align-items: center; gap: 10px; flex-wrap: wrap; }
 
    /* Finalizada (Cerrado / Cancelado): se ve apagada para distinguirla de lo pendiente */
    .card--final { background: #f8fafc; filter: grayscale(.9); opacity: .7; box-shadow: none; }
    .card--final:hover { opacity: .95; transform: none; }
    .card--final h3, .card--final .texto { color: #64748b; }
 
    @media (max-width: 720px) {
      .cabecera { flex-direction: column; }
      .indicadores { justify-content: flex-start; }
      .pie { flex-direction: column; align-items: flex-start; }
    }
  `]
})
export class SolicitudCard {
  readonly solicitud = input.required<SolicitudDetalle>();
  readonly estados = input<EstadoDto[]>([]);
  readonly agentes = input<UsuarioBasico[]>([]);
  readonly modo = input<'staff' | 'cliente'>('staff');
 
  readonly cambiarEstado = output<number>();
  readonly asignar = output<number>();
  readonly abrir = output<void>();
 
  readonly s = this.solicitud;
  readonly iniciales = iniciales;
  readonly prioridadClase = computed(() => visualPrioridad(this.solicitud().prioridadNombre).clase);
  readonly hace = computed(() => tiempoRelativo(this.solicitud().fechaCreacion));
}