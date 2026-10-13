import { Component, computed, input, output, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { UsuarioBasico } from '../../core/services/usuarios-query.service';
 
@Component({
  selector: 'app-asignacion-agente',
  standalone: true,
  imports: [MatFormFieldModule, MatSelectModule, MatButtonModule, MatIconModule],
  template: `
    <div class="asignacion">
      <mat-form-field appearance="outline" subscriptSizing="dynamic" class="campo">
        <mat-label>{{ agenteActualId() ? 'Reasignar a' : 'Asignar a' }}</mat-label>
        <mat-select [value]="seleccion()" (selectionChange)="seleccion.set($event.value)">
          @for (a of agentes(); track a.usuarioId) {
            <mat-option [value]="a.usuarioId">{{ a.nombre }}</mat-option>
          } @empty {
            <mat-option disabled>No hay agentes disponibles</mat-option>
          }
        </mat-select>
      </mat-form-field>
      <button mat-flat-button color="primary" [disabled]="!puedeAsignar()" (click)="asignar.emit(seleccion()!)">
        <mat-icon>person_add</mat-icon>{{ agenteActualId() ? 'Reasignar' : 'Asignar' }}
      </button>
    </div>`,
  styles: [`
    .asignacion { display: flex; align-items: center; gap: 8px; flex-wrap: wrap; }
    .campo { width: 200px; }
  `]
})
export class AsignacionAgente {
  readonly agentes = input<UsuarioBasico[]>([]);
  readonly agenteActualId = input<number | null>(null);
  readonly asignar = output<number>();
 
  readonly seleccion = signal<number | null>(null);
  readonly puedeAsignar = computed(() =>
    this.seleccion() !== null && this.seleccion() !== this.agenteActualId());
}