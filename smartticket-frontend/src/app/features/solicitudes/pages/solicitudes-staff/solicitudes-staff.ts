import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TokenService } from '../../../../core/services/token.service';
import { CatalogosQueryService } from '../../../../core/services/catalogos-query.service';
import { UsuariosQueryService, UsuarioBasico } from '../../../../core/services/usuarios-query.service';
import { SolicitudesQueryService } from '../../services/solicitudes-query.service';
import { SolicitudesCommandService } from '../../services/solicitudes-command.service';
import { ClientesQueryService } from '../../services/clientes-query.service';
import { SolicitudResponse } from '../../../../core/models/solicitud.models';
 
@Component({
  selector: 'app-solicitudes-staff',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatSelectModule, MatFormFieldModule, MatProgressSpinnerModule, MatSnackBarModule
  ],
  templateUrl: './solicitudes-staff.html',
  styleUrl: './solicitudes-staff.scss'
})
export class SolicitudesStaff implements OnInit {
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly solicitudesQuery = inject(SolicitudesQueryService);
  private readonly solicitudesCommand = inject(SolicitudesCommandService);
  private readonly clientesQuery = inject(ClientesQueryService);
  private readonly usuariosQuery = inject(UsuariosQueryService);
  private readonly snackBar = inject(MatSnackBar);
 
  readonly catalogos = inject(CatalogosQueryService); // público: el template lo usa
 
  readonly usuario = this.tokenService.usuarioActual;
  readonly puedeAsignar = computed(() => {
    const rol = this.usuario()?.rol;
    return rol === 'Administrador' || rol === 'Supervisor';
  });
  readonly puedeVerDashboard = this.puedeAsignar; // mismos roles
 
  readonly cargando = signal(true);
  readonly solicitudes = signal<SolicitudResponse[]>([]);
  readonly agentes = signal<UsuarioBasico[]>([]);
  readonly clientesCache = signal<Map<number, string>>(new Map());
 
  readonly filtroEstadoId = signal<number | null>(null);
  readonly filtroPrioridadId = signal<number | null>(null);
 
  readonly agenteSeleccionado = signal<Record<number, number>>({});
 
  ngOnInit(): void {
    this.catalogos.cargarCatalogos();
    this.cargarSolicitudes();
 
    if (this.puedeAsignar()) {
      this.usuariosQuery.listarPorRol('Agente').subscribe(r => this.agentes.set(r));
    }
  }
 
  cargarSolicitudes(): void {
    this.cargando.set(true);
    const estadoId = this.filtroEstadoId() ?? undefined;
    const prioridadId = this.filtroPrioridadId() ?? undefined;
 
    this.solicitudesQuery.listarTodas(estadoId, prioridadId).subscribe({
      next: (res) => {
        this.solicitudes.set(res);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }
 
  aplicarFiltros(): void {
    this.cargarSolicitudes();
  }
 
  limpiarFiltros(): void {
    this.filtroEstadoId.set(null);
    this.filtroPrioridadId.set(null);
    this.cargarSolicitudes();
  }
 
  nombreCliente(clienteId: number): string {
    const cache = this.clientesCache();
    if (!cache.has(clienteId)) {
      this.clientesQuery.obtenerPorId(clienteId).subscribe({
        next: (c) => {
          const nuevo = new Map(this.clientesCache());
          nuevo.set(clienteId, c.nombre);
          this.clientesCache.set(nuevo);
        },
        error: () => {
          const nuevo = new Map(this.clientesCache());
          nuevo.set(clienteId, `Cliente #${clienteId}`);
          this.clientesCache.set(nuevo);
        }
      });
      return 'Cargando...';
    }
    return cache.get(clienteId)!;
  }
 
  nombreAgente(usuarioId: number): string {
    return this.agentes().find(a => a.usuarioId === usuarioId)?.nombre ?? `Agente #${usuarioId}`;
  }
 
  // El backend devuelve las fechas en UTC pero sin la "Z" al leerlas de la BD,
  // y el navegador las interpretaba como hora local (5 horas de diferencia en Colombia).
  aUtc(fecha: string): string {
    return /(Z|[+-]\d{2}:\d{2})$/i.test(fecha) ? fecha : `${fecha}Z`;
  }
 
  cambiarEstado(solicitud: SolicitudResponse, nuevoEstadoId: number): void {
    const usuarioId = this.usuario()?.id;
    if (!usuarioId) return;
 
    this.solicitudesCommand.cambiarEstado(solicitud.solicitudId, { nuevoEstadoId, usuarioId }).subscribe({
      next: () => {
        this.snackBar.open(
          `Solicitud #${solicitud.solicitudId} → ${this.catalogos.nombreEstado(nuevoEstadoId)}`,
          'Cerrar', { duration: 3000 });
        this.cargarSolicitudes();
      },
      error: () => this.snackBar.open('No fue posible actualizar el estado', 'Cerrar', { duration: 3000 })
    });
  }
 
  seleccionarAgente(solicitudId: number, usuarioId: number): void {
    this.agenteSeleccionado.update(m => ({ ...m, [solicitudId]: usuarioId }));
  }
 
  guardarAsignacion(solicitud: SolicitudResponse): void {
    const usuarioAsignadoId = this.agenteSeleccionado()[solicitud.solicitudId];
    if (!usuarioAsignadoId) return;
 
    this.solicitudesCommand.asignar(solicitud.solicitudId, { usuarioAsignadoId }).subscribe({
      next: () => {
        this.snackBar.open('Solicitud asignada', 'Cerrar', { duration: 3000 });
        this.cargarSolicitudes();
      },
      error: () => this.snackBar.open('No fue posible asignar la solicitud', 'Cerrar', { duration: 3000 })
    });
  }
 
  irADashboard(): void {
    this.router.navigate(['/dashboard']);
  }
 
  cerrarSesion(): void {
    this.tokenService.cerrarSesion();
    this.router.navigate(['/auth/login']);
  }
}