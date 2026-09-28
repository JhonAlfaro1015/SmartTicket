namespace SmartTicket.Application.Services.Clasificacion.ClasificacionML;

public interface IMotorClasificacionML
{
    (string CategoriaPredicha, float Confianza) PredecirCategoria(string texto);
}
