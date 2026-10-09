namespace SmartTicket.Application.Services.Clasificacion.ClasificacionML;

public record MetricasModelo(double MicroAccuracy, double MacroAccuracy, double LogLoss);

public record ResultadoEntrenamiento(
    MetricasModelo Categoria,
    MetricasModelo Prioridad,
    int RegistrosEntrenamiento,
    int RegistrosValidacion,
    string TipoValidacion);

public interface IEntrenadorModeloML
{
    ResultadoEntrenamiento EntrenarDesdeCsv(string rutaEntrenamiento, string? rutaValidacion = null);
}