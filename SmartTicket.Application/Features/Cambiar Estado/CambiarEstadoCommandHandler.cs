using MediatR;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Cambiar_Estado;

public class CambiarEstadoCommandHandler(ISolicitudRepository solicitudRepository, IEstadoRepository estadoRepository) : IRequestHandler<CambiarEstadoCommand, Unit>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IEstadoRepository _estadoRepository = estadoRepository;

    public async Task<Unit> Handle(CambiarEstadoCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId) ?? throw new KeyNotFoundException("La solicitud no existe.");

        var nuevoEstado = await _estadoRepository.GetByIdAsync(request.NuevoEstadoId) ?? throw new ArgumentException("El estado indicado no existe.");

        await _solicitudRepository.CambiarEstadoAsync(
            request.SolicitudId,
            solicitud.EstadoId,
            nuevoEstado.EstadoId,
            request.UsuarioId,
            nuevoEstado.EsFinal);

        return Unit.Value;
    }
}
