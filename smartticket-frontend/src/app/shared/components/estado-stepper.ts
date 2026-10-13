import { Component, input, output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { visualEstado } from '../../core/constants/visual';
import { EstadoSiguiente } from '../../core/models/solicitud.models';
import { EstadoDto } from '../../core/services/catalogos-query.service';
 
@Component({
  selector: 'app-estado-stepper',
  standalone: true,
  imports: [MatIconModule, MatTooltipModule],
  template: `
    <div class="stepper" role="group" aria-label="Estado de la solicitud">
      @for (e of estados(); track e.estadoId) {
        <span class="envoltorio" [matTooltip]="tooltip(e)">
          <button type="button" class="paso" [class]="visual(e.nombre).clase"
                  [class.paso--actual]="esActual(e)"
                  [disabled]="!puedeIr(e)"
                  [attr.aria-current]="esActual(e) ? 'step' : null"
                  (click)="cambiar.emit(e.estadoId)">
            <mat-icon>{{ visual(e.nombre).icono }}</mat-icon>{{ e.nombre }}
          </button>
        </span>
      }
    </div>`,
  styles: [`
    .stepper { display: flex; flex-wrap: wrap; gap: 6px; }
    .envoltorio { display: inline-block; }
    .paso { display: inline-flex; align-items: center; gap: 4px; padding: 5px 12px 5px 8px; border-radius: 999px;
            border: 1.5px solid transparent; font: inherit; font-size: 12.5px; font-weight: 600; cursor: pointer;
            transition: transform .12s, box-shadow .12s; }
    .paso mat-icon { font-size: 16px; width: 16px; height: 16px; }
    .paso:not(:disabled) { border-color: currentColor; }
    .paso:not(:disabled):hover { transform: translateY(-1px); box-shadow: 0 2px 6px rgba(0,0,0,.15); }
    .paso--actual { box-shadow: 0 0 0 2px #fff, 0 0 0 4px currentColor; cursor: default; }
    .paso:disabled:not(.paso--actual) { background: #f1f5f9; color: #a0aab8; border-color: #e2e8f0; cursor: not-allowed; }
  `]
})
export class EstadoStepper {
  readonly estados = input<EstadoDto[]>([]);
  readonly estadoActualId = input.required<number>();
  readonly siguientes = input<EstadoSiguiente[]>([]);
  readonly cambiar = output<number>();
 
  readonly visual = visualEstado;
 
  esActual(e: EstadoDto): boolean {
    return e.estadoId === this.estadoActualId();
  }
 
  puedeIr(e: EstadoDto): boolean {
    return this.siguientes().some(s => s.estadoId === e.estadoId);
  }
 
  tooltip(e: EstadoDto): string {
    if (this.esActual(e)) return 'Estado actual';
    return this.puedeIr(e) ? 'Pasar a ' + e.nombre : 'No disponible desde el estado actual';
  }
}