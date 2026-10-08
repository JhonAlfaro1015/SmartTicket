namespace SmartTicket.Application.Services.Clasificacion.ClasificacionML;

public interface IMotorClasificacionML
{
    (string CategoriaPredicha, float Confianza) PredecirCategoria(string texto);
    (string PrioridadPredicha, float Confianza) PredecirPrioridad(string texto);
}
