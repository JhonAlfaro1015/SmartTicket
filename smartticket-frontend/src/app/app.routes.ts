import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth/login',
    loadComponent: () => import('./features/auth/pages/login-empleado/login-empleado').then(m => m.LoginEmpleado)
  },
  {
    path: 'auth/cliente/login',
    loadComponent: () => import('./features/auth/pages/cliente-login/cliente-login').then(m => m.ClienteLogin)
  },
  {
    path: 'auth/cliente/registro',
    loadComponent: () => import('./features/auth/pages/cliente-registro/cliente-registro').then(m => m.ClienteRegistro)
  },
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' }
];