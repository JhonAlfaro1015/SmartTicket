using MediatR;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Solicitudes.Asignar;

public class AsignarSolicitudCommandHandler(ISolicitudRepository solicitudRepository, IUsuarioRepository usuarioRepository) : IRequestHandler<AsignarSolicitudCommand, Unit>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

    public async Task<Unit> Handle(AsignarSolicitudCommand request, CancellationToken cancellationToken)
    {
        _ = await _solicitudRepository.GetByIdAsync(request.SolicitudId) ?? throw new KeyNotFoundException("La solicitud no existe.");

        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioAsignadoId) ?? throw new ArgumentException("El usuario a asignar no existe.");

        if (!usuario.Activo)
            throw new ArgumentException("No se puede asignar a un usuario inactivo.");

        await _solicitudRepository.AsignarAsync(request.SolicitudId, request.UsuarioAsignadoId);

        return Unit.Value;
    }
}
