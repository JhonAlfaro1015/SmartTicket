namespace SmartTicket.Application.Common;

public static class EstadosSistema
{
    public const string Nuevo = "Nuevo";
    public const string EnProceso = "En Proceso";
    public const string Resuelto = "Resuelto";
    public const string Cerrado = "Cerrado";
    public const string Cancelado = "Cancelado";

    // Trabajo abierto: el reloj del SLA corre mientras la solicitud esté aquí.
    public static readonly string[] Pendientes = { Nuevo, EnProceso };
}

public static class DashboardReglas
{
    public const int HorasEnRiesgo = 2;      // SLA que vence en menos de estas horas = "en riesgo"
    public const int DiasTendencia = 14;
    public const int CantidadUrgentes = 6;
}
