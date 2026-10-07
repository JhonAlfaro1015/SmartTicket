import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
 
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
  {
    path: 'dashboard',
    canActivate: [authGuard, roleGuard(['Administrador', 'Supervisor'])],
    loadComponent: () => import('./features/dashboard/pages/dashboard-home/dashboard-home').then(m => m.DashboardHome)
  },
  {
    path: 'solicitudes',
    canActivate: [authGuard, roleGuard(['Administrador', 'Agente', 'Supervisor'])],
    loadComponent: () => import('./features/solicitudes/pages/solicitudes-staff/solicitudes-staff').then(m => m.SolicitudesStaff)
  },
  {
    path: 'mis-solicitudes',
    canActivate: [authGuard, roleGuard(['Cliente'])],
    loadComponent: () => import('./features/solicitudes/pages/mis-solicitudes/mis-solicitudes').then(m => m.MisSolicitudes)
  },
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  { path: '**', redirectTo: 'auth/login' }
];