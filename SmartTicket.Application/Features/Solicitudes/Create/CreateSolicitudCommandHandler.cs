using AutoMapper;
using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;
using SmartTicket.Application.Services.Clasificacion;
using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Features.Solicitudes.Create;

public class CreateSolicitudCommandHandler(ISolicitudRepository solicitudRepository,
                                           IEstadoRepository estadoRepository,
                                           ICategoriaRepository categoriaRepository,
                                           IAreaRepository areaRepository,
                                           IPrioridadRepository prioridadRepository,
                                           ISubcategoriaRepository subcategoriaRepository,
                                           IMotorClasificacionTexto motorClasificacion,
                                           IMapper mapper) : IRequestHandler<CreateSolicitudCommand, SolicitudResponseDto>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IEstadoRepository _estadoRepository = estadoRepository;
    private readonly ICategoriaRepository _categoriaRepository = categoriaRepository;
    private readonly IAreaRepository _areaRepository = areaRepository;
    private readonly IPrioridadRepository _prioridadRepository = prioridadRepository;
    private readonly ISubcategoriaRepository _subcategoriaRepository = subcategoriaRepository;
    private readonly IMotorClasificacionTexto _motorClasificacion = motorClasificacion;
    private readonly IMapper _mapper = mapper;

    public async Task<SolicitudResponseDto> Handle(CreateSolicitudCommand request, CancellationToken cancellationToken)
    {
        var resultado = _motorClasificacion.Clasificar(request.TextoOriginal);

        var categoria = await _categoriaRepository.GetByNombreAsync(resultado.CategoriaNombre)
            ?? throw new InvalidOperationException($"La categoría '{resultado.CategoriaNombre}' no existe en dbo.Categorias.");

        var area = await _areaRepository.GetByNombreAsync(resultado.AreaNombre)
            ?? throw new InvalidOperationException($"El área '{resultado.AreaNombre}' no existe en dbo.Areas.");

        var prioridad = await _prioridadRepository.GetByNombreAsync(resultado.PrioridadNombre)
            ?? throw new InvalidOperationException($"La prioridad '{resultado.PrioridadNombre}' no existe en dbo.Prioridades.");

        var estadoInicial = await _estadoRepository.GetByNombreAsync("Nuevo")
            ?? throw new InvalidOperationException("No existe el estado 'Nuevo' en dbo.Estados.");

        int? subcategoriaId = null;
        if (resultado.SubcategoriaNombre is not null)
        {
            var subcategoria = await _subcategoriaRepository.GetByCategoriaAndNombreAsync(categoria.CategoriaId, resultado.SubcategoriaNombre);
            subcategoriaId = subcategoria?.SubcategoriaId; // si no está seedeada, queda null sin romper el flujo
        }

        var solicitud = new Tbl_Solicitud
        {
            ClienteId = request.ClienteId,
            CategoriaId = categoria.CategoriaId,
            SubcategoriaId = subcategoriaId,
            PrioridadId = prioridad.PrioridadId,
            AreaId = area.AreaId,
            EstadoId = estadoInicial.EstadoId,
            TextoOriginal = request.TextoOriginal,
            Puntuacion = resultado.Puntuacion,
            FechaCreacion = DateTime.UtcNow
        };

        solicitud.SolicitudId = await _solicitudRepository.CreateAsync(solicitud);

        return _mapper.Map<SolicitudResponseDto>(solicitud);
    }
}
