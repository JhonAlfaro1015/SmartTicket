namespace SmartTicket.Application.Dtos.Solicitudes;

public class EstadoSiguienteDto
{
    public int EstadoId { get; set; }
    public string Nombre { get; set; } = null!;
    public bool EsFinal { get; set; }
}

public class SolicitudDetalleDto
{
    public long SolicitudId { get; set; }

    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = null!;

    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = null!;
    public int? SubcategoriaId { get; set; }
    public string? SubcategoriaNombre { get; set; }

    public int PrioridadId { get; set; }
    public string PrioridadNombre { get; set; } = null!;
    public byte PrioridadNivel { get; set; }          // 1 = más urgente

    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = null!;

    public int EstadoId { get; set; }
    public string EstadoNombre { get; set; } = null!;
    public bool EstadoEsFinal { get; set; }

    public int? UsuarioAsignadoId { get; set; }
    public string? AgenteNombre { get; set; }

    public string TextoOriginal { get; set; } = null!;
    public decimal? Puntuacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLimiteSLA { get; set; }
    public DateTime? FechaCierre { get; set; }

    // Solo se llenan en el detalle (GET /api/solicitudes/{id}). El frontend
    // muestra los controles según estos valores y no replica ninguna regla.
    public List<EstadoSiguienteDto> EstadosSiguientes { get; set; } = new();
    public bool PuedeComentar { get; set; }
    public bool PuedeAsignar { get; set; }
    public bool SlaActivo { get; set; }
}
