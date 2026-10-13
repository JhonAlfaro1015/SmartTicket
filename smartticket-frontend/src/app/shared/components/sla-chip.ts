import { Component, computed, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { calcularSla, marcaTiempo } from '../../core/utils/fechas';
 
@Component({
  selector: 'app-sla-chip',
  standalone: true,
  imports: [MatIconModule],
  template: `
    @if (sla(); as s) {
      <span class="sla" [class]="'sla--' + s.nivel" [attr.title]="titulo()">
        <mat-icon>{{ s.nivel === 'vencido' ? 'warning' : 'schedule' }}</mat-icon>{{ s.texto }}
      </span>
    }`,
  styles: [`
    .sla { display: inline-flex; align-items: center; gap: 4px; padding: 3px 10px 3px 6px; border-radius: 8px;
           font-size: 12px; font-weight: 600; }
    .sla mat-icon { font-size: 16px; width: 16px; height: 16px; }
    .sla--ok { background: #eef2f7; color: #475569; }
    .sla--proximo { background: #fff4e0; color: #b45309; }
    .sla--vencido { background: #fde8e8; color: #b91c1c; }
  `]
})
export class SlaChip {
  readonly fechaLimite = input<string | null>(null);
  readonly esFinal = input(false);
 
  // Una solicitud finalizada ya no corre contra el reloj
  readonly sla = computed(() => (this.esFinal() ? null : calcularSla(this.fechaLimite())));
 
  readonly titulo = computed(() => {
    const f = this.fechaLimite();
    return f ? 'Límite: ' + new Date(marcaTiempo(f)).toLocaleString('es-CO', { dateStyle: 'short', timeStyle: 'short' }) : '';
  });
}