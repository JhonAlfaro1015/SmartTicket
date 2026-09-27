using AutoMapper;
using MediatR;
using SmartTicket.Application.Dtos.Clientes;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Features.Clientes.Create;

public class CreateClienteCommandHandler(IClienteRepository clienteRepository, IMapper mapper) : IRequestHandler<CreateClienteCommand, ClienteResponseDto>
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ClienteResponseDto> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = new Tbl_Cliente
        {
            Nombre = request.Nombre,
            Email = request.Email,
            Telefono = request.Telefono,
            FechaCreacion = DateTime.UtcNow
        };

        cliente.ClienteId = await _clienteRepository.CreateAsync(cliente);

        return _mapper.Map<ClienteResponseDto>(cliente);
    }
}
