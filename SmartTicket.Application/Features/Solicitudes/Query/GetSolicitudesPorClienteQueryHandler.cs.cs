using AutoMapper;
using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Solicitudes.Query;

public class GetSolicitudesPorClienteQueryHandler : IRequestHandler<GetSolicitudesPorClienteQuery, IEnumerable<SolicitudResponseDto>>
{
    private readonly ISolicitudRepository _solicitudRepository;
    private readonly IMapper _mapper;

    public GetSolicitudesPorClienteQueryHandler(ISolicitudRepository solicitudRepository, IMapper mapper)
    {
        _solicitudRepository = solicitudRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SolicitudResponseDto>> Handle(GetSolicitudesPorClienteQuery request, CancellationToken cancellationToken)
    {
        var solicitudes = await _solicitudRepository.GetByClienteIdAsync(request.ClienteId);
        return _mapper.Map<IEnumerable<SolicitudResponseDto>>(solicitudes);
    }
}
