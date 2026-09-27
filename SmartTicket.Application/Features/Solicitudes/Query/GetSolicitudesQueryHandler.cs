using AutoMapper;
using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;


namespace SmartTicket.Application.Features.Solicitudes.Query;

public class GetSolicitudesQueryHandler(ISolicitudRepository solicitudRepository, IMapper mapper) : IRequestHandler<GetSolicitudesQuery, IEnumerable<SolicitudResponseDto>>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<SolicitudResponseDto>> Handle(GetSolicitudesQuery request, CancellationToken cancellationToken)
    {
        var solicitudes = await _solicitudRepository.GetAllAsync(request.EstadoId, request.PrioridadId);
        return _mapper.Map<IEnumerable<SolicitudResponseDto>>(solicitudes);
    }
}
