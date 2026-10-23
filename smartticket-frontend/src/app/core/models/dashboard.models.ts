import { SolicitudDetalle } from './solicitud.models';
 
export interface Conteo {
  nombre: string;
  cantidad: number;
}
 
export interface AgenteCarga {
  usuarioId: number;
  nombre: string;
  pendientes: number;
  resueltas: number;
  total: number;
}
 
export interface PuntoTendencia {
  fecha: string;
  creadas: number;
  finalizadas: number;
}
 
export interface DashboardResumen {
  totalSolicitudes: number;
  porEstado: Conteo[];
  porPrioridad: Conteo[];
  porCategoria: Conteo[];
  tiempoPromedioAtencionHoras: number | null;
 
  pendientes: number;
  sinAsignar: number;
  vencidas: number;
  enRiesgo: number;
  horasEnRiesgo: number;
  creadasUltimos7Dias: number;
  creadasPrevios7Dias: number;
  resueltas: number;
  tiempoPromedioResolucionHoras: number | null;
  cumplimientoSlaPorcentaje: number | null;
  porAgente: AgenteCarga[];
  tendencia: PuntoTendencia[];
  urgentes: SolicitudDetalle[];
}