using MediatR;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Features.Clientes.Auth;

public class RegisterClienteCommandHandler(
    IClienteRepository clienteRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RegisterClienteCommand, AuthResponseDto>
{
    private const string RolCliente = "Cliente";

    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<AuthResponseDto> Handle(RegisterClienteCommand request, CancellationToken cancellationToken)
    {
        var existente = await _clienteRepository.GetByEmailAsync(request.Email);
        if (existente is not null)
            throw new InvalidOperationException("Ya existe un cliente registrado con ese correo.");

        var cliente = new Tbl_Cliente
        {
            Nombre = request.Nombre,
            Email = request.Email,
            Telefono = request.Telefono,
            PasswordHash = _passwordHasher.Hash(request.Password),
            FechaCreacion = DateTime.UtcNow
        };

        cliente.ClienteId = await _clienteRepository.CreateAsync(cliente);

        var (token, expiraEn) = _jwtTokenGenerator.GenerateToken(cliente.ClienteId, cliente.Email!, cliente.Nombre, RolCliente);

        return new AuthResponseDto
        {
            Token = token,
            Nombre = cliente.Nombre,
            Email = cliente.Email!,
            Rol = RolCliente,
            ExpiraEn = expiraEn
        };
    }
}
