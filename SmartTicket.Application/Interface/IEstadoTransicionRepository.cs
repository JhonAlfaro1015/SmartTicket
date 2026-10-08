namespace SmartTicket.Application.Interface;

public interface IEstadoTransicionRepository
{
    Task<bool> EsPermitidaAsync(int estadoOrigenId, int estadoDestinoId);
}