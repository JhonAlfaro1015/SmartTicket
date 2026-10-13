export interface SolicitudResponse {
  solicitudId: number;
  clienteId: number;
  categoriaId: number;
  subcategoriaId: number | null;
  prioridadId: number;
  areaId: number;
  estadoId: number;
  usuarioAsignadoId: number | null;
  textoOriginal: string;
  puntuacion: number | null;
  fechaCreacion: string;
  fechaLimiteSLA: string | null;
  fechaCierre: string | null;
}
 
export interface CreateSolicitudRequest {
  textoOriginal: string;
}
 
export interface CambiarEstadoRequest {
  nuevoEstadoId: number;           // quién cambia el estado sale del token, ya no se envía
}
 
export interface AsignarSolicitudRequest {
  usuarioAsignadoId: number;
}
 
export interface EstadoSiguiente {
  estadoId: number;
  nombre: string;
  esFinal: boolean;
}
 
export interface SolicitudDetalle {
  solicitudId: number;
  clienteId: number;
  clienteNombre: string;
  categoriaId: number;
  categoriaNombre: string;
  subcategoriaId: number | null;
  subcategoriaNombre: string | null;
  prioridadId: number;
  prioridadNombre: string;
  prioridadNivel: number;
  areaId: number;
  areaNombre: string;
  estadoId: number;
  estadoNombre: string;
  estadoEsFinal: boolean;
  usuarioAsignadoId: number | null;
  agenteNombre: string | null;
  textoOriginal: string;
  puntuacion: number | null;
  fechaCreacion: string;
  fechaLimiteSLA: string | null;
  fechaCierre: string | null;
  estadosSiguientes: EstadoSiguiente[];   // los llena el servidor según rol y asignación
  puedeComentar: boolean;
  puedeAsignar: boolean;
}
 
export interface Comentario {
  comentarioId: number;
  solicitudId: number;
  texto: string;
  fechaCreacion: string;
  autorNombre: string;
  autorRol: string;
  esCliente: boolean;
}
 
export interface EventoHistorial {
  historialId: number;
  solicitudId: number;
  estadoAnterior: string | null;
  estadoNuevo: string;
  usuarioNombre: string;
  usuarioRol: string;
  fechaCambio: string;
}