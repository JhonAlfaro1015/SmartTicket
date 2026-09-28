using MediatR;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;
using SmartTicket.Application.Interface;

namespace SmartTicket.Application.Features.Auth.Login;

public class LoginCommandHandler(IUsuarioRepository usuarioRepository, IRolRepository rolRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
    private readonly IRolRepository _rolRepository = rolRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("El usuario está inactivo.");

        if (!_passwordHasher.Verify(request.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var rol = await _rolRepository.GetByIdAsync(usuario.RolId);
        var rolNombre = rol?.Nombre ?? string.Empty;

        var (token, expiraEn) = _jwtTokenGenerator.GenerateToken(usuario.UsuarioId, usuario.Email, usuario.Nombre, rolNombre);

        return new AuthResponseDto
        {
            Token = token,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = rolNombre,
            ExpiraEn = expiraEn
        };
    }
}
