import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData } from 'chart.js';
import { TokenService } from '../../../../core/services/token.service';
import { DashboardQueryService } from '../../../solicitudes/services/dashboard-query.service';
import { DashboardResumen } from '../../../../core/models/dashboard.models';
 
@Component({
  selector: 'app-dashboard-home',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, BaseChartDirective
  ],
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss'
})
export class DashboardHome implements OnInit {
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly dashboardQuery = inject(DashboardQueryService);
 
  readonly usuario = this.tokenService.usuarioActual;
  readonly cargando = signal(true);
  readonly resumen = signal<DashboardResumen | null>(null);
 
  readonly chartCategoriaData = signal<ChartData<'bar'>>({ labels: [], datasets: [] });
  readonly chartPrioridadData = signal<ChartData<'doughnut'>>({ labels: [], datasets: [] });
  readonly chartEstadoData = signal<ChartData<'bar'>>({ labels: [], datasets: [] });
 
  readonly chartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { display: false } }
  };
 
  readonly doughnutOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'bottom' } }
  };
 
  ngOnInit(): void {
    this.cargarResumen();
  }
 
  cargarResumen(): void {
    this.cargando.set(true);
    this.dashboardQuery.obtenerResumen().subscribe({
      next: (res) => {
        this.resumen.set(res);
        this.construirGraficos(res);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }
 
  private construirGraficos(res: DashboardResumen): void {
    this.chartCategoriaData.set({
      labels: res.porCategoria.map(c => c.nombre),
      datasets: [{ data: res.porCategoria.map(c => c.cantidad), label: 'Solicitudes', backgroundColor: '#2d5f8a' }]
    });
 
    this.chartPrioridadData.set({
      labels: res.porPrioridad.map(p => p.nombre),
      datasets: [{
        data: res.porPrioridad.map(p => p.cantidad),
        backgroundColor: ['#b3261e', '#e65100', '#2d5f8a', '#4caf50']
      }]
    });
 
    this.chartEstadoData.set({
      labels: res.porEstado.map(e => e.nombre),
      datasets: [{ data: res.porEstado.map(e => e.cantidad), label: 'Solicitudes', backgroundColor: '#0f766e' }]
    });
  }
 
  irASolicitudes(): void {
    this.router.navigate(['/solicitudes']);
  }
 
  cerrarSesion(): void {
    this.tokenService.cerrarSesion();
    this.router.navigate(['/auth/login']);
  }
}