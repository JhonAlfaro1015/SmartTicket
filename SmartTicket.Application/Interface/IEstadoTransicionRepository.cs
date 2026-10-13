using SmartTicket.Application.Dtos.Solicitudes;

namespace SmartTicket.Application.Interface;

public interface IEstadoTransicionRepository
{
    Task<bool> EsPermitidaAsync(int estadoOrigenId, int estadoDestinoId);
    Task<IEnumerable<EstadoSiguienteDto>> GetDestinosAsync(int estadoOrigenId);
}