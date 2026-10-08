using MediatR;

namespace SmartTicket.Application.Dtos.Comentarios.Agregar;

public record AgregarComentarioCommand(long SolicitudId, string Texto, int AutorId, string Rol) : IRequest<ComentarioDto>;
