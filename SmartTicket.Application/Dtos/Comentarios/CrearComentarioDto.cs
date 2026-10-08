using System.ComponentModel.DataAnnotations;
using SmartTicket.Application.Common;

namespace SmartTicket.Application.Dtos.Comentarios;

public class CrearComentarioDto
{
    [Required(ErrorMessage = "El comentario no puede estar vacío.")]
    [StringLength(ComentarioReglas.LongitudMaxima, ErrorMessage = "El comentario es demasiado largo.")]
    public string Texto { get; set; } = null!;
}