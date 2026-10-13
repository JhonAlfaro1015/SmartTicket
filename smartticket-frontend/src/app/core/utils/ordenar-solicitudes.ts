import { SolicitudDetalle } from '../models/solicitud.models';
import { marcaTiempo } from './fechas';
 
export type CriterioOrden = 'urgencia' | 'recientes';
 
export function ordenarSolicitudes(lista: readonly SolicitudDetalle[], criterio: CriterioOrden): SolicitudDetalle[] {
  const copia = [...lista];
  const masReciente = (a: SolicitudDetalle, b: SolicitudDetalle) =>
    marcaTiempo(b.fechaCreacion) - marcaTiempo(a.fechaCreacion);
 
  if (criterio === 'recientes') return copia.sort(masReciente);
 
  // Urgencia: prioridad (1 = más urgente), luego el SLA que vence primero, luego la más reciente
  const vence = (s: SolicitudDetalle) =>
    s.fechaLimiteSLA ? marcaTiempo(s.fechaLimiteSLA) : Number.MAX_SAFE_INTEGER;
 
  return copia.sort((a, b) =>
    a.prioridadNivel - b.prioridadNivel || vence(a) - vence(b) || masReciente(a, b));
}