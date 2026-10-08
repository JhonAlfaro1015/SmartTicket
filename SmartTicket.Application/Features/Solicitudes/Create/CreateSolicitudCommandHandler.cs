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
    IReglaSlaRepository reglaSlaRepository,
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
    private readonly IReglaSlaRepository _reglaSlaRepository = reglaSlaRepository;
    private readonly IMotorClasificacionTexto _motorReglas = motorReglas;
    private readonly IMotorClasificacionML _motorML = motorML;
    private readonly IMapper _mapper = mapper;

    public async Task<SolicitudResponseDto> Handle(CreateSolicitudCommand request, CancellationToken cancellationToken)
    {
        // 1) ML.NET interpreta el texto: categoría y prioridad
        var (categoriaPredicha, _) = _motorML.PredecirCategoria(request.TextoOriginal);
        var (prioridadPredicha, _) = _motorML.PredecirPrioridad(request.TextoOriginal);

        // 2) Reglas de negocio: puntaje de urgencia y subcategoría
        var resultadoReglas = _motorReglas.Clasificar(request.TextoOriginal);

        var categoria = await _categoriaRepository.GetByNombreAsync(categoriaPredicha)
            ?? throw new InvalidOperationException($"La categoría '{categoriaPredicha}' no existe en dbo.Categorias.");

        // El área corresponde a la categoría predicha (en tu esquema comparten nombre)
        var area = await _areaRepository.GetByNombreAsync(categoriaPredicha)
            ?? throw new InvalidOperationException($"El área '{categoriaPredicha}' no existe en dbo.Areas.");

        var prioridadML = await _prioridadRepository.GetByNombreAsync(prioridadPredicha);
        var prioridadReglas = await _prioridadRepository.GetByNombreAsync(resultadoReglas.PrioridadNombre)
            ?? throw new InvalidOperationException($"La prioridad '{resultadoReglas.PrioridadNombre}' no existe en dbo.Prioridades.");

        // NivelUrgencia: 1 = más urgente. Gana la más urgente de las dos.
        var prioridad = prioridadML is null || prioridadReglas.NivelUrgencia < prioridadML.NivelUrgencia
            ? prioridadReglas
            : prioridadML;

        var estadoInicial = await _estadoRepository.GetByNombreAsync("Nuevo")
            ?? throw new InvalidOperationException("No existe el estado 'Nuevo' en dbo.Estados.");

        int? subcategoriaId = null;
        if (resultadoReglas.SubcategoriaNombre is not null)
        {
            var subcategoria = await _subcategoriaRepository.GetByCategoriaAndNombreAsync(
                categoria.CategoriaId, resultadoReglas.SubcategoriaNombre);
            subcategoriaId = subcategoria?.SubcategoriaId;
        }

        // 3) SLA: horas según ReglasSLA (por subcategoría o, si no hay, por prioridad)
        var horasSla = await _reglaSlaRepository.GetHorasSlaAsync(subcategoriaId, prioridad.PrioridadId);
        var ahora = DateTime.UtcNow;

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
            FechaCreacion = ahora,
            FechaLimiteSLA = horasSla.HasValue ? ahora.AddHours(horasSla.Value) : null
        };

        solicitud.SolicitudId = await _solicitudRepository.CreateAsync(solicitud);

        return _mapper.Map<SolicitudResponseDto>(solicitud);
    }
}
