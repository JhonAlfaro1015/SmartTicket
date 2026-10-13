namespace SmartTicket.Application.Services.Clasificacion.ClasificacionML;

public record ErrorClasificacion(string Texto, string Esperada, string Predicha);

public record MetricasValidacion(
    string Archivo,
    int Registros,
    double MicroAccuracy,
    double MacroAccuracy,
    double LogLoss,
    double ExactitudDentroDeUnNivel,   // en categoría coincide con MicroAccuracy
    List<ErrorClasificacion> Errores);

public record ResultadoEntrenamiento(
    int RegistrosEntrenamiento,
    List<MetricasValidacion> Categoria,
    List<MetricasValidacion> Prioridad);

public interface IEntrenadorModeloML
{
    ResultadoEntrenamiento EntrenarDesdeCsv(string rutaEntrenamiento, IReadOnlyList<string> rutasValidacion);
}