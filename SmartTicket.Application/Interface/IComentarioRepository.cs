using SmartTicket.Application.Dtos.Comentarios;

namespace SmartTicket.Application.Interface;

public interface IComentarioRepository
{
    Task<long> AgregarAsync(long solicitudId, int? usuarioId, int? clienteId, string texto);
    Task<ComentarioDto?> ObtenerPorIdAsync(long comentarioId);
    Task<IEnumerable<ComentarioDto>> ListarPorSolicitudAsync(long solicitudId);
}
