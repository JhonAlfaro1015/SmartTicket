import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { ClienteAuthCommandService } from '../../services/cliente-auth-command.service';
import { TokenService } from '../../../../core/services/token.service';
 
@Component({
  selector: 'app-cliente-registro',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule,
    MatProgressSpinnerModule, MatIconModule
  ],
  templateUrl: './cliente-registro.html',
  styleUrl: './cliente-registro.scss'
})
export class ClienteRegistro {
  private readonly fb = inject(FormBuilder);
  private readonly clienteAuthService = inject(ClienteAuthCommandService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
 
  readonly cargando = signal(false);
  readonly errorMensaje = signal<string | null>(null);
 
  readonly formulario = this.fb.nonNullable.group({
    nombre: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    telefono: [''],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });
 
  onSubmit(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }
 
    this.cargando.set(true);
    this.errorMensaje.set(null);
 
    this.clienteAuthService.registrar(this.formulario.getRawValue()).subscribe({
      next: (respuesta) => {
        this.tokenService.guardarToken(respuesta.token);
        this.cargando.set(false);
        this.router.navigate(['/mis-solicitudes']);
      },
      error: (err) => {
        this.cargando.set(false);
        this.errorMensaje.set(
          err.status === 409 ? 'Ya existe una cuenta con ese correo.' : 'No fue posible completar el registro.'
        );
      }
    });
  }
}