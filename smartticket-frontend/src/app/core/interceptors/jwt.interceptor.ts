import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TokenService } from '../services/token.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const token = tokenService.obtenerToken();

  if (!token) {
    return next(req);
  }

  const reqConToken = req.clone({
    setHeaders: { Authorization: `Bearer ${token}` }
  });

  return next(reqConToken);
};