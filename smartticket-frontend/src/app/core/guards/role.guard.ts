import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokenService } from '../services/token.service';

export function roleGuard(rolesPermitidos: string[]): CanActivateFn {
  return () => {
    const tokenService = inject(TokenService);
    const router = inject(Router);
    const usuario = tokenService.usuarioActual();

    if (usuario && rolesPermitidos.includes(usuario.rol)) {
      return true;
    }

    router.navigate(['/']);
    return false;
  };
}