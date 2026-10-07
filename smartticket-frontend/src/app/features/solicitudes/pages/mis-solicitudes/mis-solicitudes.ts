import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TokenService } from '../../../../core/services/token.service';
import { CatalogosQueryService } from '../../../../core/services/catalogos-query.service';
import { SolicitudesCommandService } from '../../services/solicitudes-command.service';
import { SolicitudesQueryService } from '../../services/solicitudes-query.service';
import { SolicitudResponse } from '../../../../core/models/solicitud.models';
 
@Component({
  selector: 'app-mis-solicitudes',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatProgressSpinnerModule, MatSnackBarModule
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
 
  readonly catalogos = inject(CatalogosQueryService); // público: el template lo usa
 
  readonly usuario = this.tokenService.usuarioActual;
  readonly cargandoLista = signal(true);
  readonly enviando = signal(false);
  readonly solicitudes = signal<SolicitudResponse[]>([]);
 
  readonly formulario = this.fb.nonNullable.group({
    textoOriginal: ['', [Validators.required, Validators.minLength(10)]]
  });
 
  ngOnInit(): void {
    this.catalogos.cargarCatalogos();
    this.cargarMisSolicitudes();
  }
 
  cargarMisSolicitudes(): void {
    this.cargandoLista.set(true);
    this.solicitudesQuery.listarMias().subscribe({
      next: (res) => {
        this.solicitudes.set(res);
        this.cargandoLista.set(false);
      },
      error: () => this.cargandoLista.set(false)
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
        this.snackBar.open('Solicitud creada y clasificada correctamente', 'Cerrar', { duration: 4000 });
        this.cargarMisSolicitudes();
      },
      error: () => {
        this.enviando.set(false);
        this.snackBar.open('No fue posible crear la solicitud', 'Cerrar', { duration: 4000 });
      }
    });
  }
 
  cerrarSesion(): void {
    this.tokenService.cerrarSesion();
    this.router.navigate(['/auth/cliente/login']);
  }
}