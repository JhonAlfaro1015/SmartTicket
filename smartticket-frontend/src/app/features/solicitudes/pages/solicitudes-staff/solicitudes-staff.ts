import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MENSAJES, mensajeDeError } from '../../../../core/constants/mensajes';
import { ROLES, ROLES_GESTION } from '../../../../core/constants/roles';
import { visualEstado } from '../../../../core/constants/visual';
import { SolicitudDetalle } from '../../../../core/models/solicitud.models';
import { CatalogosQueryService } from '../../../../core/services/catalogos-query.service';
import { TokenService } from '../../../../core/services/token.service';
import { UsuariosQueryService, UsuarioBasico } from '../../../../core/services/usuarios-query.service';
import { CriterioOrden, ordenarSolicitudes } from '../../../../core/utils/ordenar-solicitudes';
import { normalizar } from '../../../../core/utils/texto';
import { SolicitudCard } from '../../components/solicitud-card';
import { SolicitudesCommandService } from '../../services/solicitudes-command.service';
import { SolicitudesQueryService } from '../../services/solicitudes-query.service';
 
@Component({
  selector: 'app-solicitudes-staff',
  standalone: true,
  imports: [
    CommonModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatIconModule, MatInputModule,
    MatProgressSpinnerModule, MatSelectModule, MatSlideToggleModule, MatSnackBarModule, SolicitudCard
  ],
  templateUrl: './solicitudes-staff.html',
  styleUrl: './solicitudes-staff.scss'
})
export class SolicitudesStaff implements OnInit {
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly solicitudesQuery = inject(SolicitudesQueryService);
  private readonly solicitudesCommand = inject(SolicitudesCommandService);
  private readonly usuariosQuery = inject(UsuariosQueryService);
  private readonly snackBar = inject(MatSnackBar);
 
  readonly catalogos = inject(CatalogosQueryService);
  readonly visualEstado = visualEstado;
 
  readonly usuario = this.tokenService.usuarioActual;
  readonly esGestion = computed(() => ROLES_GESTION.includes(this.usuario()?.rol ?? ''));
  readonly esAgente = computed(() => this.usuario()?.rol === ROLES.Agente);
 
  readonly cargando = signal(true);
  readonly solicitudes = signal<SolicitudDetalle[]>([]);
  readonly agentes = signal<UsuarioBasico[]>([]);
 
  readonly busqueda = signal('');
  readonly filtroEstadoId = signal<number | null>(null);
  readonly filtroPrioridadId = signal<number | null>(null);
  readonly orden = signal<CriterioOrden>('urgencia');
  readonly mostrarFinalizadas = signal(true);
 
  private readonly filtradas = computed(() => {
    const texto = normalizar(this.busqueda().trim());
    const estadoId = this.filtroEstadoId();
    const prioridadId = this.filtroPrioridadId();
 
    return this.solicitudes().filter(s =>
      (estadoId === null || s.estadoId === estadoId) &&
      (prioridadId === null || s.prioridadId === prioridadId) &&
      (texto === '' ||
        normalizar(s.solicitudId + ' ' + s.clienteNombre + ' ' + s.textoOriginal + ' ' + s.categoriaNombre)
          .includes(texto)));
  });
 
  readonly pendientes = computed(() =>
    ordenarSolicitudes(this.filtradas().filter(s => !s.estadoEsFinal), this.orden()));
 
  readonly finalizadas = computed(() =>
    ordenarSolicitudes(this.filtradas().filter(s => s.estadoEsFinal), 'recientes'));
 
  readonly conteoPorEstado = computed(() =>
    this.catalogos.estados().map(estado => ({
      estado,
      cantidad: this.solicitudes().filter(s => s.estadoId === estado.estadoId).length
    })));
 
  ngOnInit(): void {
    this.catalogos.cargarCatalogos();
    this.cargar();
 
    if (this.esGestion()) {
      this.usuariosQuery.listarPorRol(ROLES.Agente).subscribe(a => this.agentes.set(a));
    }
  }
 
  cargar(): void {
    this.cargando.set(true);
    this.solicitudesQuery.listarTodas().subscribe({
      next: (lista) => {
        this.solicitudes.set(lista);
        this.cargando.set(false);
      },
      error: (err) => {
        this.cargando.set(false);
        this.avisar(mensajeDeError(err, MENSAJES.errorCargar));
      }
    });
  }
 
  alternarEstado(estadoId: number): void {
    this.filtroEstadoId.update(actual => (actual === estadoId ? null : estadoId));
  }
 
  onCambiarEstado(solicitud: SolicitudDetalle, nuevoEstadoId: number): void {
    this.solicitudesCommand.cambiarEstado(solicitud.solicitudId, { nuevoEstadoId }).subscribe({
      next: () => {
        this.avisar(MENSAJES.estadoActualizado(solicitud.solicitudId, this.nombreEstado(nuevoEstadoId)));
        this.cargar();
      },
      error: (err) => this.avisar(mensajeDeError(err, MENSAJES.errorEstado))
    });
  }
 
  onAsignar(solicitud: SolicitudDetalle, usuarioAsignadoId: number): void {
    this.solicitudesCommand.asignar(solicitud.solicitudId, { usuarioAsignadoId }).subscribe({
      next: () => {
        this.avisar(MENSAJES.asignada);
        this.cargar();
      },
      error: (err) => this.avisar(mensajeDeError(err, MENSAJES.errorAsignar))
    });
  }
 
  abrir(solicitud: SolicitudDetalle): void {
    this.router.navigate(['/solicitudes', solicitud.solicitudId]);
  }
 
  irADashboard(): void {
    this.router.navigate(['/dashboard']);
  }
 
  cerrarSesion(): void {
    this.tokenService.cerrarSesion();
    this.router.navigate(['/auth/login']);
  }
 
  private nombreEstado(estadoId: number): string {
    return this.catalogos.nombreEstado(estadoId);
  }
 
  private avisar(mensaje: string): void {
    this.snackBar.open(mensaje, 'Cerrar', { duration: 4000 });
  }
}