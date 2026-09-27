using AutoMapper;
using MediatR;
using SmartTicket.Application.Dtos.Clientes;
using SmartTicket.Application.Interface;


namespace SmartTicket.Application.Features.Clientes.Query;

public class GetClienteByIdQueryHandler(IClienteRepository clienteRepository, IMapper mapper) : IRequestHandler<GetClienteByIdQuery, ClienteResponseDto?>
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ClienteResponseDto?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
        return cliente is null ? null : _mapper.Map<ClienteResponseDto>(cliente);
    }
}
