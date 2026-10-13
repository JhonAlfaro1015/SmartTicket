import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MENSAJES, mensajeDeError } from '../../../../core/constants/mensajes';
import { SolicitudDetalle } from '../../../../core/models/solicitud.models';
import { CatalogosQueryService } from '../../../../core/services/catalogos-query.service';
import { TokenService } from '../../../../core/services/token.service';
import { ordenarSolicitudes } from '../../../../core/utils/ordenar-solicitudes';
import { SolicitudCard } from '../../components/solicitud-card';
import { SolicitudesCommandService } from '../../services/solicitudes-command.service';
import { SolicitudesQueryService } from '../../services/solicitudes-query.service';
 
@Component({
  selector: 'app-mis-solicitudes',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatIconModule,
    MatInputModule, MatProgressSpinnerModule, MatSnackBarModule, SolicitudCard
  ],
  templateUrl: './mis-solicitudes.html',
  styleUrl: './mis-solicitudes.scss'
})
export class MisSolicitudes implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly solicitudesCommand = inject(SolicitudesCommandService);
  private readonly solicitudesQuery = inject(SolicitudesQueryService);
  private readonly snackBar = inject(MatSnackBar);
 
  readonly catalogos = inject(CatalogosQueryService);
  readonly usuario = this.tokenService.usuarioActual;
 
  readonly cargando = signal(true);
  readonly enviando = signal(false);
  readonly solicitudes = signal<SolicitudDetalle[]>([]);
 
  readonly enCurso = computed(() =>
    ordenarSolicitudes(this.solicitudes().filter(s => !s.estadoEsFinal), 'recientes'));
  readonly finalizadas = computed(() =>
    ordenarSolicitudes(this.solicitudes().filter(s => s.estadoEsFinal), 'recientes'));
 
  readonly formulario = this.fb.nonNullable.group({
    textoOriginal: ['', [Validators.required, Validators.minLength(10)]]
  });
 
  ngOnInit(): void {
    this.catalogos.cargarCatalogos();
    this.cargar();
  }
 
  cargar(): void {
    this.cargando.set(true);
    this.solicitudesQuery.listarMias().subscribe({
      next: (lista) => {
        this.solicitudes.set(lista);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }
 
  onSubmit(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }
 
    this.enviando.set(true);
    this.solicitudesCommand.crear(this.formulario.getRawValue()).subscribe({
      next: () => {
        this.enviando.set(false);
        this.formulario.reset();
        this.snackBar.open(MENSAJES.solicitudCreada, 'Cerrar', { duration: 4000 });
        this.cargar();
      },
      error: (err) => {
        this.enviando.set(false);
        this.snackBar.open(mensajeDeError(err, MENSAJES.errorCrear), 'Cerrar', { duration: 4000 });
      }
    });
  }
 
  abrir(solicitud: SolicitudDetalle): void {
    this.router.navigate(['/mis-solicitudes', solicitud.solicitudId]);
  }
 
  cerrarSesion(): void {
    this.tokenService.cerrarSesion();
    this.router.navigate(['/auth/cliente/login']);
  }
}