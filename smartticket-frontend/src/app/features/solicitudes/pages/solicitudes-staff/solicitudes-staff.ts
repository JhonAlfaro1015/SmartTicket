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
 
  readonly estadoSeleccionado = signal<Record<number, number>>({});
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
 
  seleccionarEstado(solicitudId: number, estadoId: number): void {
    this.estadoSeleccionado.update(m => ({ ...m, [solicitudId]: estadoId }));
  }
 
  seleccionarAgente(solicitudId: number, usuarioId: number): void {
    this.agenteSeleccionado.update(m => ({ ...m, [solicitudId]: usuarioId }));
  }
 
  guardarEstado(solicitud: SolicitudResponse): void {
    const nuevoEstadoId = this.estadoSeleccionado()[solicitud.solicitudId];
    const usuarioId = this.usuario()?.id;
    if (!nuevoEstadoId || !usuarioId) return;
 
    this.solicitudesCommand.cambiarEstado(solicitud.solicitudId, { nuevoEstadoId, usuarioId }).subscribe({
      next: () => {
        this.snackBar.open('Estado actualizado', 'Cerrar', { duration: 3000 });
        this.cargarSolicitudes();
      },
      error: () => this.snackBar.open('No fue posible actualizar el estado', 'Cerrar', { duration: 3000 })
    });
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