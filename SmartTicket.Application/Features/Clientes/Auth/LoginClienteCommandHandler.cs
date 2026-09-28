using MediatR;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;
using SmartTicket.Application.Dtos.Auth;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Clientes.Auth;

public class LoginClienteCommandHandler(IClienteRepository clienteRepository,IPasswordHasher passwordHasher,IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginClienteCommand, AuthResponseDto>
{
    private const string RolCliente = "Cliente";

    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<AuthResponseDto> Handle(LoginClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (string.IsNullOrEmpty(cliente.PasswordHash) || !_passwordHasher.Verify(request.Password, cliente.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

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
