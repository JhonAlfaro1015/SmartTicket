using MediatR;
using SmartTicket.Application.Dtos.Comentarios;

namespace SmartTicket.Application.Features.Comentarios.Listar;

public record GetComentariosQuery(long SolicitudId, int UsuarioId, string Rol) : IRequest<IEnumerable<ComentarioDto>>;
