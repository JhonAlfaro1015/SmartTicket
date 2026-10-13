using MediatR;
using SmartTicket.Application.Common.Exceptions;
using SmartTicket.Application.Common;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Cambiar_Estado;

public class CambiarEstadoCommandHandler(
    ISolicitudRepository solicitudRepository,
    IEstadoRepository estadoRepository,
    IEstadoTransicionRepository transicionRepository) : IRequestHandler<CambiarEstadoCommand, Unit>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IEstadoRepository _estadoRepository = estadoRepository;
    private readonly IEstadoTransicionRepository _transicionRepository = transicionRepository;

    public async Task<Unit> Handle(CambiarEstadoCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId)
            ?? throw new RecursoNoEncontradoException("La solicitud no existe.");

        // El Agente solo actúa sobre lo que tiene asignado
        if (!PermisosSolicitud.PuedeCambiarEstado(request.Rol, request.UsuarioId, solicitud.UsuarioAsignadoId))
            throw new AccesoDenegadoException("No tienes permiso para cambiar el estado de esta solicitud.");

        var estadoActual = await _estadoRepository.GetByIdAsync(solicitud.EstadoId)
            ?? throw new RecursoNoEncontradoException("El estado actual de la solicitud no existe.");

        var nuevoEstado = await _estadoRepository.GetByIdAsync(request.NuevoEstadoId)
            ?? throw new RecursoNoEncontradoException("El estado indicado no existe.");

        if (estadoActual.EstadoId == nuevoEstado.EstadoId)
            throw new ReglaNegocioException($"La solicitud ya está en estado '{estadoActual.Nombre}'.");

        if (!await _transicionRepository.EsPermitidaAsync(estadoActual.EstadoId, nuevoEstado.EstadoId))
            throw new ReglaNegocioException(
                $"No se permite pasar de '{estadoActual.Nombre}' a '{nuevoEstado.Nombre}'.");

        await _solicitudRepository.CambiarEstadoAsync(
            request.SolicitudId,
            estadoActual.EstadoId,
            nuevoEstado.EstadoId,
            request.UsuarioId,
            nuevoEstado.EsFinal);

        return Unit.Value;
    }
}
