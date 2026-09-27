namespace SmartTicket.Application.Services.Clasificacion;

public interface IMotorClasificacionTexto
{
    ResultadoClasificacion Clasificar(string texto);
}
