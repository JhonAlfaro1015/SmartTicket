import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { COMENTARIO_MAX, MENSAJES, mensajeDeError } from '../../../../core/constants/mensajes';
import { ROLES } from '../../../../core/constants/roles';
import { Comentario, EventoHistorial, SolicitudDetalle } from '../../../../core/models/solicitud.models';
import { CatalogosQueryService } from '../../../../core/services/catalogos-query.service';
import { TokenService } from '../../../../core/services/token.service';
import { UsuariosQueryService, UsuarioBasico } from '../../../../core/services/usuarios-query.service';
import { aUtc, marcaTiempo } from '../../../../core/utils/fechas';
import { iniciales } from '../../../../core/utils/texto';
import { AsignacionAgente } from '../../../../shared/components/asignacion-agente';
import { EstadoBadge } from '../../../../shared/components/estado-badge';
import { EstadoStepper } from '../../../../shared/components/estado-stepper';
import { PrioridadBadge } from '../../../../shared/components/prioridad-badge';
import { ScoreMeter } from '../../../../shared/components/score-meter';
import { SlaChip } from '../../../../shared/components/sla-chip';
import { ComentariosCommandService } from '../../services/comentarios-command.service';
import { ComentariosQueryService } from '../../services/comentarios-query.service';
import { SolicitudesCommandService } from '../../services/solicitudes-command.service';
import { SolicitudesQueryService } from '../../services/solicitudes-query.service';
 
interface ItemTimeline {
  tipo: 'cliente' | 'personal' | 'sistema';
  fecha: string;
  autor: string;
  rol?: string;
  texto?: string;
  desde?: string | null;
  hasta?: string;
}
 
@Component({
  selector: 'app-solicitud-detalle',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatIconModule,
    MatInputModule, MatProgressSpinnerModule, MatSnackBarModule, AsignacionAgente, EstadoBadge,
    EstadoStepper, PrioridadBadge, ScoreMeter, SlaChip
  ],
  templateUrl: './solicitud-detalle.html',
  styleUrl: './solicitud-detalle.scss'
})
export class SolicitudDetallePage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly tokenService = inject(TokenService);
  private readonly solicitudesQuery = inject(SolicitudesQueryService);
  private readonly solicitudesCommand = inject(SolicitudesCommandService);
  private readonly comentariosQuery = inject(ComentariosQueryService);
  private readonly comentariosCommand = inject(ComentariosCommandService);
  private readonly usuariosQuery = inject(UsuariosQueryService);
  private readonly snackBar = inject(MatSnackBar);
 
  readonly catalogos = inject(CatalogosQueryService);
  readonly id = Number(this.route.snapshot.paramMap.get('id'));
  readonly COMENTARIO_MAX = COMENTARIO_MAX;
  readonly aUtc = aUtc;
  readonly iniciales = iniciales;
 
  readonly esCliente = computed(() => this.tokenService.usuarioActual()?.rol === ROLES.Cliente);
 
  readonly cargando = signal(true);
  readonly error = signal<string | null>(null);
  readonly enviando = signal(false);
  readonly solicitud = signal<SolicitudDetalle | null>(null);
  readonly comentarios = signal<Comentario[]>([]);
  readonly historial = signal<EventoHistorial[]>([]);
  readonly agentes = signal<UsuarioBasico[]>([]);
 
  readonly formulario = this.fb.nonNullable.group({
    texto: ['', [Validators.required, Validators.maxLength(COMENTARIO_MAX)]]
  });
 
  // Conversación + cambios de estado, en orden cronológico
  readonly eventos = computed<ItemTimeline[]>(() => {
    const s = this.solicitud();
    if (!s) return [];
 
    const items: ItemTimeline[] = [
      { tipo: 'cliente', fecha: s.fechaCreacion, autor: s.clienteNombre, rol: ROLES.Cliente, texto: s.textoOriginal },
      ...this.comentarios().map(c => ({
        tipo: (c.esCliente ? 'cliente' : 'personal') as ItemTimeline['tipo'],
        fecha: c.fechaCreacion, autor: c.autorNombre, rol: c.autorRol, texto: c.texto
      })),
      ...this.historial().map(h => ({
        tipo: 'sistema' as const,
        fecha: h.fechaCambio, autor: h.usuarioNombre, desde: h.estadoAnterior, hasta: h.estadoNuevo
      }))
    ];
 
    return items.sort((a, b) => marcaTiempo(a.fecha) - marcaTiempo(b.fecha));
  });
 
  ngOnInit(): void {
    this.catalogos.cargarCatalogos();
    this.cargarTodo(true);
  }
 
  cargarTodo(conSpinner: boolean): void {
    if (conSpinner) this.cargando.set(true);
    this.error.set(null);
 
    forkJoin({
      detalle: this.solicitudesQuery.obtenerPorId(this.id),
      comentarios: this.comentariosQuery.listar(this.id),
      historial: this.solicitudesQuery.obtenerHistorial(this.id)
    }).subscribe({
      next: ({ detalle, comentarios, historial }) => {
        this.solicitud.set(detalle);
        this.comentarios.set(comentarios);
        this.historial.set(historial);
        this.cargando.set(false);
 
        if (detalle.puedeAsignar && this.agentes().length === 0) {
          this.usuariosQuery.listarPorRol(ROLES.Agente).subscribe(a => this.agentes.set(a));
        }
      },
      error: (err) => {
        this.cargando.set(false);
        this.error.set(err.status === 403 || err.status === 404
          ? MENSAJES.sinAcceso
          : mensajeDeError(err, MENSAJES.errorCargar));
      }
    });
  }
 
  cambiarEstado(nuevoEstadoId: number): void {
    this.solicitudesCommand.cambiarEstado(this.id, { nuevoEstadoId }).subscribe({
      next: () => {
        this.avisar(MENSAJES.estadoActualizado(this.id, this.catalogos.nombreEstado(nuevoEstadoId)));
        this.cargarTodo(false);
      },
      error: (err) => this.avisar(mensajeDeError(err, MENSAJES.errorEstado))
    });
  }
 
  asignar(usuarioAsignadoId: number): void {
    this.solicitudesCommand.asignar(this.id, { usuarioAsignadoId }).subscribe({
      next: () => {
        this.avisar(MENSAJES.asignada);
        this.cargarTodo(false);
      },
      error: (err) => this.avisar(mensajeDeError(err, MENSAJES.errorAsignar))
    });
  }
 
  publicar(): void {
    const texto = this.formulario.controls.texto.value.trim();
    if (!texto || this.enviando()) return;
 
    this.enviando.set(true);
    this.comentariosCommand.publicar(this.id, texto).subscribe({
      next: (comentario) => {
        this.comentarios.update(lista => [...lista, comentario]);
        this.formulario.reset();
        this.enviando.set(false);
        this.avisar(MENSAJES.comentarioPublicado);
      },
      error: (err) => {
        this.enviando.set(false);
        this.avisar(mensajeDeError(err, MENSAJES.errorComentario));
      }
    });
  }
 
  volver(): void {
    this.router.navigate([this.esCliente() ? '/mis-solicitudes' : '/solicitudes']);
  }
 
  private avisar(mensaje: string): void {
    this.snackBar.open(mensaje, 'Cerrar', { duration: 4000 });
  }
}