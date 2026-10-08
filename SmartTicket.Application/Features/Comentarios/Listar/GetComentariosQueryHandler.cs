using MediatR;
using SmartTicket.Application.Common.Exceptions;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Comentarios;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Comentarios.Listar;

public class GetComentariosQueryHandler(
    ISolicitudRepository solicitudRepository,
    IComentarioRepository comentarioRepository) : IRequestHandler<GetComentariosQuery, IEnumerable<ComentarioDto>>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IComentarioRepository _comentarioRepository = comentarioRepository;

    public async Task<IEnumerable<ComentarioDto>> Handle(GetComentariosQuery request, CancellationToken cancellationToken)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId)
            ?? throw new RecursoNoEncontradoException("La solicitud no existe.");

        // Un cliente solo ve la conversación de sus propias solicitudes
        if (request.Rol == RolesSistema.Cliente && solicitud.ClienteId != request.UsuarioId)
            throw new AccesoDenegadoException("Solo puedes ver los comentarios de tus propias solicitudes.");

        return await _comentarioRepository.ListarPorSolicitudAsync(request.SolicitudId);
    }
}