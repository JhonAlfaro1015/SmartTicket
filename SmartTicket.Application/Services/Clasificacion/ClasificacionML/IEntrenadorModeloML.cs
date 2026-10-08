namespace SmartTicket.Application.Services.Clasificacion.ClasificacionML;

public record MetricasModelo(double MicroAccuracy, double MacroAccuracy, double LogLoss);

public record ResultadoEntrenamiento(MetricasModelo Categoria, MetricasModelo Prioridad, int TotalRegistros);

public interface IEntrenadorModeloML
{
    ResultadoEntrenamiento EntrenarDesdeCsv(string rutaCsv);
}