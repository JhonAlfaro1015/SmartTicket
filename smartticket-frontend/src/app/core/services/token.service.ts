import { Injectable, signal } from '@angular/core';

export interface UsuarioActual {
  id: number;
  email: string;
  nombre: string;
  rol: string;
}

const TOKEN_KEY = 'smartticket_token';

// Estos son los nombres de claim REALES que tu JwtTokenGenerator escribe
// en el token (ClaimTypes.Name y ClaimTypes.Role en .NET se serializan como
// estas URIs largas, no como "name"/"role" simples).
const CLAIM_NAME = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const CLAIM_ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

@Injectable({ providedIn: 'root' })
export class TokenService {
  // Signal reactivo: cualquier componente puede leer quién está logueado
  // y la UI se actualiza sola cuando cambia (login/logout).
  readonly usuarioActual = signal<UsuarioActual | null>(this.decodificarTokenGuardado());

  guardarToken(token: string): void {
    // sessionStorage en vez de localStorage: el token se borra al cerrar
    // el navegador, reduciendo la ventana de riesgo si el equipo es compartido.
    sessionStorage.setItem(TOKEN_KEY, token);
    this.usuarioActual.set(this.decodificarToken(token));
  }

  obtenerToken(): string | null {
    return sessionStorage.getItem(TOKEN_KEY);
  }

  cerrarSesion(): void {
    sessionStorage.removeItem(TOKEN_KEY);
    this.usuarioActual.set(null);
  }

  estaAutenticado(): boolean {
    return this.obtenerToken() !== null;
  }

  private decodificarTokenGuardado(): UsuarioActual | null {
    const token = this.obtenerToken();
    return token ? this.decodificarToken(token) : null;
  }

  private decodificarToken(token: string): UsuarioActual | null {
    try {
      const payloadBase64 = token.split('.')[1];
      const payloadJson = atob(payloadBase64.replace(/-/g, '+').replace(/_/g, '/'));
      const payload = JSON.parse(payloadJson);

      return {
        id: Number(payload['sub']),
        email: payload['email'],
        nombre: payload[CLAIM_NAME],
        rol: payload[CLAIM_ROLE]
      };
    } catch {
      return null;
    }
  }
}