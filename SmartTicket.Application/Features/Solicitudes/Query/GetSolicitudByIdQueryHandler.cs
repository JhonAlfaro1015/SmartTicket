using AutoMapper;
using MediatR;
using SmartTicket.Application.Dtos.Solicitudes;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Solicitudes.Query;

public class GetSolicitudByIdQueryHandler(ISolicitudRepository solicitudRepository, IMapper mapper) : IRequestHandler<GetSolicitudByIdQuery, SolicitudResponseDto?>
{
    private readonly ISolicitudRepository _solicitudRepository = solicitudRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<SolicitudResponseDto?> Handle(GetSolicitudByIdQuery request, CancellationToken cancellationToken)
    {
        var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId);
        return solicitud is null ? null : _mapper.Map<SolicitudResponseDto>(solicitud);
    }
}
