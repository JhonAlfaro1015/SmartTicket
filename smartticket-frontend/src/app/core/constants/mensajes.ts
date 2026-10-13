export const COMENTARIO_MAX = 2000;
 
export const MENSAJES = {
  errorCargar: 'No fue posible cargar la información',
  sinAcceso: 'No tienes acceso a esta solicitud',
  errorEstado: 'No fue posible actualizar el estado',
  asignada: 'Solicitud asignada',
  errorAsignar: 'No fue posible asignar la solicitud',
  comentarioPublicado: 'Comentario publicado',
  errorComentario: 'No fue posible publicar el comentario',
  solicitudCreada: 'Solicitud creada y clasificada correctamente',
  errorCrear: 'No fue posible crear la solicitud',
  estadoActualizado: (id: number, estado: string) => 'Solicitud #' + id + ' → ' + estado
};
 
// Usa el mensaje que manda el backend ({ message }) y, si no hay, el de respaldo.
export function mensajeDeError(err: unknown, respaldo: string): string {
  const mensaje = (err as { error?: { message?: string } })?.error?.message;
  return mensaje ?? respaldo;
}