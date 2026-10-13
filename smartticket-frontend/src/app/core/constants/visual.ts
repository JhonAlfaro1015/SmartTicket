export interface VisualEstado { icono: string; clase: string; }
export interface VisualPrioridad { icono: string; clase: string; etiqueta: string; }
 
const VISUAL_ESTADO: Record<string, VisualEstado> = {
  'Nuevo': { icono: 'fiber_new', clase: 'estado--nuevo' },
  'En Proceso': { icono: 'autorenew', clase: 'estado--proceso' },
  'Resuelto': { icono: 'check_circle', clase: 'estado--resuelto' },
  'Cerrado': { icono: 'lock', clase: 'estado--cerrado' },
  'Cancelado': { icono: 'cancel', clase: 'estado--cancelado' }
};
 
export function visualEstado(nombre: string): VisualEstado {
  return VISUAL_ESTADO[nombre] ?? { icono: 'label', clase: 'estado--defecto' };
}
 
const VISUAL_PRIORIDAD: Record<string, VisualPrioridad> = {
  CRITICA: { icono: 'error', clase: 'prioridad--critica', etiqueta: 'CRÍTICA' },
  ALTA: { icono: 'arrow_upward', clase: 'prioridad--alta', etiqueta: 'ALTA' },
  MEDIA: { icono: 'remove', clase: 'prioridad--media', etiqueta: 'MEDIA' },
  BAJA: { icono: 'arrow_downward', clase: 'prioridad--baja', etiqueta: 'BAJA' }
};
 
export function visualPrioridad(nombre: string): VisualPrioridad {
  return VISUAL_PRIORIDAD[nombre] ?? { icono: 'flag', clase: 'prioridad--defecto', etiqueta: nombre };
}
 
// Solo colorea el anillo del score (los cortes coinciden con los del motor de reglas).
export function colorScore(valor: number): string {
  if (valor >= 85) return 'var(--pr-critica)';
  if (valor >= 60) return 'var(--pr-alta)';
  if (valor >= 30) return 'var(--pr-media)';
  return 'var(--pr-baja)';
}