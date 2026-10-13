import { Component, computed, input } from '@angular/core';
import { colorScore } from '../../core/constants/visual';
 
@Component({
  selector: 'app-score-meter',
  standalone: true,
  template: `
    @if (valor() !== null) {
      <div class="contenedor" [attr.title]="'Puntaje de urgencia: ' + valor() + ' de 100'">
        <div class="anillo" [style.--p]="porcentaje()" [style.--c]="color()"><span>{{ valor() }}</span></div>
        <small>Score</small>
      </div>
    }`,
  styles: [`
    .contenedor { display: inline-flex; flex-direction: column; align-items: center; gap: 2px; }
    .anillo { position: relative; width: 46px; height: 46px; border-radius: 50%; display: grid; place-items: center;
              background: conic-gradient(var(--c) calc(var(--p) * 1%), #e5e7eb 0); }
    .anillo::before { content: ''; position: absolute; inset: 5px; border-radius: 50%; background: #fff; }
    .anillo span { position: relative; font-weight: 700; font-size: 13px; color: #1f2937; }
    small { font-size: 10px; letter-spacing: .4px; text-transform: uppercase; color: #6b7280; }
  `]
})
export class ScoreMeter {
  readonly valor = input<number | null>(null);
  readonly porcentaje = computed(() => Math.min(100, Math.max(0, this.valor() ?? 0)));
  readonly color = computed(() => colorScore(this.valor() ?? 0));
}