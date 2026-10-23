import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData } from 'chart.js';
import { colorHexEstado, colorHexPrioridad } from '../../../../core/constants/visual';
import { DashboardResumen } from '../../../../core/models/dashboard.models';
import { TokenService } from '../../../../core/services/token.service';
import { formatearHoras } from '../../../../core/utils/fechas';
import { iniciales } from '../../../../core/utils/texto';
import { EstadoBadge } from '../../../../shared/components/estado-badge';
import { PrioridadBadge } from '../../../../shared/components/prioridad-badge';
import { SlaChip } from '../../../../shared/components/sla-chip';
import { DashboardQueryService } from '../../../solicitudes/services/dashboard-query.service';
 
interface Kpi {
  icono: string;
  etiqueta: string;
  valor: string;
  detalle?: string;
  claseDetalle?: string;
  tono: 'azul' | 'ambar' | 'rojo' | 'violeta' | 'verde' | 'gris';
}
 
@Component({
  selector: 'app-dashboard-home',
  standalone: true,
  imports: [
    CommonModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, BaseChartDirective,
    EstadoBadge, PrioridadBadge, SlaChip
  ],
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss'
})
export class DashboardHome implements OnInit {
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly dashboardQuery = inject(DashboardQueryService);
  private readonly destroyRef = inject(DestroyRef);
 
  readonly usuario = this.tokenService.usuarioActual;
  readonly iniciales = iniciales;
  readonly fechaHoy = new Date().toLocaleDateString('es-CO', {
    weekday: 'long', day: 'numeric', month: 'long', year: 'numeric'
  });
 
  readonly cargando = signal(true);
  readonly error = signal(false);
  readonly resumen = signal<DashboardResumen | null>(null);
  readonly actualizado = signal<Date | null>(null);
 
  readonly textoActualizado = computed(() =>
    this.actualizado()?.toLocaleTimeString('es-CO', { hour: '2-digit', minute: '2-digit' }) ?? '');
 
  // ---------- Indicadores ----------
  readonly kpis = computed<Kpi[]>(() => {
    const r = this.resumen();
    if (!r) return [];
 
    const delta = r.creadasUltimos7Dias - r.creadasPrevios7Dias;
    const pct = r.creadasPrevios7Dias > 0 ? Math.round((delta / r.creadasPrevios7Dias) * 100) : null;
    const tendencia =
      pct === null ? (r.creadasUltimos7Dias > 0 ? 'Sin datos de la semana previa' : 'Sin actividad reciente')
      : delta === 0 ? 'Igual que los 7 días previos'
      : (delta > 0 ? '▲ ' : '▼ ') + Math.abs(pct) + ' % vs 7 días previos';
 
    return [
      { icono: 'inbox', etiqueta: 'Total de solicitudes', valor: String(r.totalSolicitudes), tono: 'azul' },
      { icono: 'pending_actions', etiqueta: 'Pendientes', valor: String(r.pendientes), detalle: 'Trabajo abierto', tono: 'ambar' },
      {
        icono: 'alarm', etiqueta: 'SLA vencido', valor: String(r.vencidas), tono: 'rojo',
        detalle: r.vencidas > 0 ? 'Requieren atención ya' : 'Sin vencimientos',
        claseDetalle: r.vencidas > 0 ? 'detalle--alerta' : ''
      },
      { icono: 'hourglass_top', etiqueta: 'En riesgo (≤ ' + r.horasEnRiesgo + ' h)', valor: String(r.enRiesgo), detalle: 'Vencen pronto', tono: 'ambar' },
      {
        icono: 'person_off', etiqueta: 'Sin asignar', valor: String(r.sinAsignar), tono: 'violeta',
        detalle: r.sinAsignar > 0 ? 'Esperan un agente' : 'Todo asignado'
      },
      {
        icono: 'verified', etiqueta: 'Cumplimiento SLA', tono: 'verde',
        valor: r.cumplimientoSlaPorcentaje === null ? '—' : String(r.cumplimientoSlaPorcentaje).replace('.', ',') + ' %',
        detalle: 'Sobre ' + r.resueltas + ' resueltas'
      },
      { icono: 'timer', etiqueta: 'Tiempo medio de resolución', valor: formatearHoras(r.tiempoPromedioResolucionHoras), detalle: 'Creación → Resuelto', tono: 'azul' },
      { icono: 'trending_up', etiqueta: 'Creadas en 7 días', valor: String(r.creadasUltimos7Dias), detalle: tendencia, tono: 'gris' }
    ];
  });
 
  // ---------- Carga por agente ----------
  private readonly maximoAgente = computed(() =>
    Math.max(1, ...(this.resumen()?.porAgente.map(a => a.total) ?? [1]), this.resumen()?.sinAsignar ?? 0));
 
  readonly cargaAgentes = computed(() => {
    const maximo = this.maximoAgente();
    return (this.resumen()?.porAgente ?? []).map(a => ({
      ...a,
      pctPendientes: (a.pendientes / maximo) * 100,
      pctResueltas: (a.resueltas / maximo) * 100
    }));
  });
 
  readonly sinAsignarPct = computed(() => ((this.resumen()?.sinAsignar ?? 0) / this.maximoAgente()) * 100);
 
  // ---------- Gráficos ----------
  readonly datosEstados = computed<ChartData<'doughnut'>>(() => {
    const lista = this.resumen()?.porEstado ?? [];
    return {
      labels: lista.map(e => e.nombre),
      datasets: [{
        data: lista.map(e => e.cantidad),
        backgroundColor: lista.map(e => colorHexEstado(e.nombre)),
        borderColor: '#fff', borderWidth: 3
      }]
    };
  });
 
  readonly datosPrioridades = computed<ChartData<'bar'>>(() => {
    const lista = this.resumen()?.porPrioridad ?? [];
    return {
      labels: lista.map(p => p.nombre),
      datasets: [{
        data: lista.map(p => p.cantidad),
        backgroundColor: lista.map(p => colorHexPrioridad(p.nombre)),
        borderRadius: 8, maxBarThickness: 28
      }]
    };
  });
 
  readonly datosCategorias = computed<ChartData<'bar'>>(() => {
    const lista = this.resumen()?.porCategoria ?? [];
    return {
      labels: lista.map(c => c.nombre),
      datasets: [{ data: lista.map(c => c.cantidad), backgroundColor: '#3b82f6', borderRadius: 8, maxBarThickness: 28 }]
    };
  });
 
  readonly datosTendencia = computed<ChartData<'line'>>(() => {
    const puntos = this.resumen()?.tendencia ?? [];
    // La fecha llega como "2026-10-13T00:00:00": se corta el texto para no depender de zonas horarias
    const etiqueta = (f: string) => f.slice(8, 10) + '/' + f.slice(5, 7);
    return {
      labels: puntos.map(p => etiqueta(p.fecha)),
      datasets: [
        {
          label: 'Creadas', data: puntos.map(p => p.creadas), borderColor: '#2563eb',
          backgroundColor: 'rgba(37, 99, 235, .12)', fill: true, tension: .35, pointRadius: 3, borderWidth: 2.5
        },
        {
          label: 'Finalizadas', data: puntos.map(p => p.finalizadas), borderColor: '#16a34a',
          backgroundColor: 'rgba(22, 163, 74, .10)', fill: true, tension: .35, pointRadius: 3, borderWidth: 2.5
        }
      ]
    };
  });
 
  readonly opcionesDona: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true, maintainAspectRatio: false, cutout: '68%',
    plugins: { legend: { position: 'right', labels: { usePointStyle: true, boxWidth: 8, padding: 14 } } }
  };
 
  readonly opcionesBarraHorizontal: ChartConfiguration<'bar'>['options'] = {
    indexAxis: 'y', responsive: true, maintainAspectRatio: false,
    plugins: { legend: { display: false } },
    scales: {
      x: { beginAtZero: true, ticks: { precision: 0 }, grid: { color: '#eef2f6' } },
      y: { grid: { display: false } }
    }
  };
 
  readonly opcionesLinea: ChartConfiguration<'line'>['options'] = {
    responsive: true, maintainAspectRatio: false,
    interaction: { mode: 'index', intersect: false },
    plugins: { legend: { position: 'top', align: 'end', labels: { usePointStyle: true, boxWidth: 8 } } },
    scales: {
      y: { beginAtZero: true, ticks: { precision: 0 }, grid: { color: '#eef2f6' } },
      x: { grid: { display: false } }
    }
  };
 
  ngOnInit(): void {
    this.cargar(true);
 
    // El panel se refresca solo cada minuto
    const temporizador = setInterval(() => this.cargar(false), 60_000);
    this.destroyRef.onDestroy(() => clearInterval(temporizador));
  }
 
  cargar(conSpinner: boolean): void {
    if (conSpinner) this.cargando.set(true);
    this.error.set(false);
 
    this.dashboardQuery.obtenerResumen().subscribe({
      next: (resumen) => {
        this.resumen.set(resumen);
        this.actualizado.set(new Date());
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        if (!this.resumen()) this.error.set(true);   // si ya había datos, se conservan
      }
    });
  }
 
  abrir(solicitudId: number): void {
    this.router.navigate(['/solicitudes', solicitudId]);
  }
 
  irASolicitudes(): void {
    this.router.navigate(['/solicitudes']);
  }
 
  cerrarSesion(): void {
    this.tokenService.cerrarSesion();
    this.router.navigate(['/auth/login']);
  }
}