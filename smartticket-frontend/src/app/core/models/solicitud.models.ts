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
  nuevoEstadoId: number;
  usuarioId: number;
}

export interface AsignarSolicitudRequest {
  usuarioAsignadoId: number;
}