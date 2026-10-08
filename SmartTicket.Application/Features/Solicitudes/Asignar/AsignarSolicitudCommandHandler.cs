using MediatR;
using SmartTicket.Application.Common.Exceptions;
using SmartTicket.Application.Common;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Solicitudes.Asignar;

public class AsignarSolicitudCommandHandler(
    ISolicitudRepository solicitudRepository,
    IUsuarioRepository usuarioRepository,
    IRolRepository rolRepository,
    IEstadoRepository estadoRepository) : IRequestHandler<AsignarSolicitudCommand, Unit>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
    private readonly IRolRepository _rolRepository = rolRepository;
    private readonly IEstadoRepository _estadoRepository = estadoRepository;

    public async Task<Unit> Handle(AsignarSolicitudCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId)
            ?? throw new RecursoNoEncontradoException("La solicitud no existe.");

        var estado = await _estadoRepository.GetByIdAsync(solicitud.EstadoId);
        if (estado is { EsFinal: true })
            throw new ReglaNegocioException("No se puede asignar una solicitud cerrada o cancelada.");

        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioAsignadoId)
            ?? throw new RecursoNoEncontradoException("El usuario a asignar no existe.");

        if (!usuario.Activo)
            throw new ReglaNegocioException("No se puede asignar a un usuario inactivo.");

        var rol = await _rolRepository.GetByIdAsync(usuario.RolId);
        if (rol?.Nombre != RolesSistema.Agente)
            throw new ReglaNegocioException("Solo se puede asignar a usuarios con rol Agente.");

        await _solicitudRepository.AsignarAsync(request.SolicitudId, request.UsuarioAsignadoId);

        return Unit.Value;
    }
}
