export interface Conteo {
  nombre: string;
  cantidad: number;
}

export interface DashboardResumen {
  totalSolicitudes: number;
  porEstado: Conteo[];
  porPrioridad: Conteo[];
  porCategoria: Conteo[];
  tiempoPromedioAtencionHoras: number | null;
}