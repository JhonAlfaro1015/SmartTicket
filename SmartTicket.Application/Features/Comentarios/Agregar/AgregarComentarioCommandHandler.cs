using MediatR;
using SmartTicket.Application.Common.Exceptions;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Comentarios.Agregar;
using SmartTicket.Application.Dtos.Comentarios;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Comentarios.Agregar;

public class AgregarComentarioCommandHandler(
    ISolicitudRepository solicitudRepository,
    IEstadoRepository estadoRepository,
    IComentarioRepository comentarioRepository) : IRequestHandler<AgregarComentarioCommand, ComentarioDto>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IEstadoRepository _estadoRepository = estadoRepository;
    private readonly IComentarioRepository _comentarioRepository = comentarioRepository;

    public async Task<ComentarioDto> Handle(AgregarComentarioCommand request, CancellationToken cancellationToken)
    {
        var texto = request.Texto?.Trim();

        if (string.IsNullOrWhiteSpace(texto))
            throw new ReglaNegocioException("El comentario no puede estar vacío.");

        if (texto.Length > ComentarioReglas.LongitudMaxima)
            throw new ReglaNegocioException(
                $"El comentario no puede superar {ComentarioReglas.LongitudMaxima} caracteres.");

        var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId)
            ?? throw new RecursoNoEncontradoException("La solicitud no existe.");

        var esCliente = request.Rol == RolesSistema.Cliente;

        if (esCliente && solicitud.ClienteId != request.AutorId)
            throw new AccesoDenegadoException("Solo puedes comentar tus propias solicitudes.");

        if (request.Rol == RolesSistema.Agente && solicitud.UsuarioAsignadoId != request.AutorId)
            throw new AccesoDenegadoException("Solo puedes comentar las solicitudes asignadas a ti.");

        var estado = await _estadoRepository.GetByIdAsync(solicitud.EstadoId);
        if (estado is { EsFinal: true })
            throw new ReglaNegocioException("No se pueden agregar comentarios a una solicitud cerrada o cancelada.");

        var comentarioId = await _comentarioRepository.AgregarAsync(
            request.SolicitudId,
            usuarioId: esCliente ? null : request.AutorId,
            clienteId: esCliente ? request.AutorId : null,
            texto);

        return (await _comentarioRepository.ObtenerPorIdAsync(comentarioId))!;
    }
}
