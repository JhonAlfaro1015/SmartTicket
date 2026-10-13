import { Component, computed, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { visualPrioridad } from '../../core/constants/visual';
 
@Component({
  selector: 'app-prioridad-badge',
  standalone: true,
  imports: [MatIconModule],
  template: `
    <span class="badge" [class]="visual().clase" [class.grande]="grande()">
      <mat-icon>{{ visual().icono }}</mat-icon>{{ visual().etiqueta }}
    </span>`,
  styles: [`
    .badge { display: inline-flex; align-items: center; gap: 4px; padding: 3px 10px 3px 6px;
             border-radius: 999px; font-size: 12px; font-weight: 700; letter-spacing: .3px;
             background: var(--pr-color); color: #fff; white-space: nowrap; }
    .badge mat-icon { font-size: 16px; width: 16px; height: 16px; }
    .badge.grande { font-size: 14px; padding: 5px 14px 5px 10px; }
    .badge.grande mat-icon { font-size: 18px; width: 18px; height: 18px; }
  `]
})
export class PrioridadBadge {
  readonly nombre = input.required<string>();
  readonly grande = input(false);
  readonly visual = computed(() => visualPrioridad(this.nombre()));
}