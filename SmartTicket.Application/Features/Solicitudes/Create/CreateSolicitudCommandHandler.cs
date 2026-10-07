using AutoMapper;
using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;
using SmartTicket.Application.Services.Clasificacion;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;
using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Features.Solicitudes.Create;

public class CreateSolicitudCommandHandler(
    ISolicitudRepository solicitudRepository,
    IEstadoRepository estadoRepository,
    ICategoriaRepository categoriaRepository,
    IAreaRepository areaRepository,
    IPrioridadRepository prioridadRepository,
    ISubcategoriaRepository subcategoriaRepository,
    IMotorClasificacionTexto motorReglas,
    IMotorClasificacionML motorML,
    IMapper mapper) : IRequestHandler<CreateSolicitudCommand, SolicitudResponseDto>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IEstadoRepository _estadoRepository = estadoRepository;
    private readonly ICategoriaRepository _categoriaRepository = categoriaRepository;
    private readonly IAreaRepository _areaRepository = areaRepository;
    private readonly IPrioridadRepository _prioridadRepository = prioridadRepository;
    private readonly ISubcategoriaRepository _subcategoriaRepository = subcategoriaRepository;
    private readonly IMotorClasificacionTexto _motorReglas = motorReglas;
    private readonly IMotorClasificacionML _motorML = motorML;
    private readonly IMapper _mapper = mapper;

    public async Task<SolicitudResponseDto> Handle(CreateSolicitudCommand request, CancellationToken cancellationToken)
    {
        // 1) ML.NET decide la categoría (aprendizaje supervisado, entrenado con el dataset)
        var (categoriaPredicha, _) = _motorML.PredecirCategoria(request.TextoOriginal);

        // 2) El motor de reglas de negocio decide prioridad, puntuación y subcategoría
        var resultadoReglas = _motorReglas.Clasificar(request.TextoOriginal);

        var categoria = await _categoriaRepository.GetByNombreAsync(categoriaPredicha)
            ?? throw new InvalidOperationException($"La categoría '{categoriaPredicha}' no existe en dbo.Categorias.");

        // Área usa el mismo nombre que la categoría predicha por ML (igual que en el motor de reglas)
        var area = await _areaRepository.GetByNombreAsync(categoriaPredicha)
            ?? throw new InvalidOperationException($"El área '{categoriaPredicha}' no existe en dbo.Areas.");

        var prioridad = await _prioridadRepository.GetByNombreAsync(resultadoReglas.PrioridadNombre)
            ?? throw new InvalidOperationException($"La prioridad '{resultadoReglas.PrioridadNombre}' no existe en dbo.Prioridades.");

        var estadoInicial = await _estadoRepository.GetByNombreAsync("Nuevo")
            ?? throw new InvalidOperationException("No existe el estado 'Nuevo' en dbo.Estados.");

        int? subcategoriaId = null;
        if (resultadoReglas.SubcategoriaNombre is not null)
        {
            var subcategoria = await _subcategoriaRepository.GetByCategoriaAndNombreAsync(categoria.CategoriaId, resultadoReglas.SubcategoriaNombre);
            subcategoriaId = subcategoria?.SubcategoriaId;
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
            Puntuacion = resultadoReglas.Puntuacion,
            FechaCreacion = DateTime.UtcNow
        };

        solicitud.SolicitudId = await _solicitudRepository.CreateAsync(solicitud);

        return _mapper.Map<SolicitudResponseDto>(solicitud);
    }
}
