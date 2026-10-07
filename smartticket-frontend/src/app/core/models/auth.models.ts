export interface AuthResponse {
  token: string;
  nombre: string;
  email: string;
  rol: string;
  expiraEn: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

// Registro de EMPLEADOS (Administrador/Agente/Supervisor) — /api/auth
export interface RegisterRequest {
  nombre: string;
  email: string;
  password: string;
  rolNombre: string;
}

// Registro/login de CLIENTES — /api/cliente-auth
export interface ClienteRegisterRequest {
  nombre: string;
  email: string;
  telefono?: string;
  password: string;
}

export interface ClienteLoginRequest {
  email: string;
  password: string;
}