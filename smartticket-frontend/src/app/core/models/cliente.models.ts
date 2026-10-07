export interface ClienteResponse {
  clienteId: number;
  nombre: string;
  email: string;
  telefono: string | null;
  fechaCreacion: string;
}