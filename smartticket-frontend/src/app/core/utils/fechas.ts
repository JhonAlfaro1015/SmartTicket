const MS_MIN = 60_000;
const MS_HORA = 3_600_000;
const MS_DIA = 86_400_000;
 
export const UMBRAL_SLA_PROXIMO_MS = 2 * MS_HORA;
 
export function aUtc(fecha: string): string {
  return /(Z|[+-]\d{2}:\d{2})$/i.test(fecha) ? fecha : fecha + 'Z';
}
 
export function marcaTiempo(fecha: string): number {
  return new Date(aUtc(fecha)).getTime();
}
 
export function formatearDuracion(ms: number): string {
  const abs = Math.abs(ms);
  if (abs < MS_HORA) return Math.max(1, Math.round(abs / MS_MIN)) + ' min';
  if (abs < MS_DIA) return Math.round(abs / MS_HORA) + ' h';
  return Math.round(abs / MS_DIA) + ' d';
}
 
export function tiempoRelativo(fecha: string, ahora = Date.now()): string {
  const diff = ahora - marcaTiempo(fecha);
  return diff < MS_MIN ? 'justo ahora' : 'hace ' + formatearDuracion(diff);
}
 
export interface EstadoSla { nivel: 'ok' | 'proximo' | 'vencido'; texto: string; }
 
export function calcularSla(fechaLimite: string | null, ahora = Date.now()): EstadoSla | null {
  if (!fechaLimite) return null;
  const diff = marcaTiempo(fechaLimite) - ahora;
  if (diff < 0) return { nivel: 'vencido', texto: 'Vencida hace ' + formatearDuracion(diff) };
  return {
    nivel: diff <= UMBRAL_SLA_PROXIMO_MS ? 'proximo' : 'ok',
    texto: 'Vence en ' + formatearDuracion(diff)
  };
}

export function formatearHoras(horas: number | null): string {
  if (horas === null || horas === undefined) return '—';
  if (horas < 1) return Math.max(1, Math.round(horas * 60)) + ' min';
  if (horas < 48) return (Math.round(horas * 10) / 10).toString().replace('.', ',') + ' h';
  return (Math.round((horas / 24) * 10) / 10).toString().replace('.', ',') + ' d';
}