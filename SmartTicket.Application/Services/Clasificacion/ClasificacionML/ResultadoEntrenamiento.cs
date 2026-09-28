namespace SmartTicket.Application.Services.Clasificacion.ClasificacionML;

public record ResultadoEntrenamiento(double MicroAccuracy, double MacroAccuracy, double LogLoss, int TotalRegistros);

public interface IEntrenadorModeloML
{
    ResultadoEntrenamiento EntrenarDesdeCsv(string rutaCsv);
}
