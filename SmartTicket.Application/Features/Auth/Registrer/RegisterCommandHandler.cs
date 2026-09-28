using MediatR;
using SmartTicket.Application.Dtos.Auth.LoginRequestDto;
using SmartTicket.Application.Interface;
using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Features.Auth.Registrer;

public class RegisterCommandHandler(IUsuarioRepository usuarioRepository,IRolRepository rolRepository,IPasswordHasher passwordHasher,IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
    private readonly IRolRepository _rolRepository = rolRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(request.Email);
        if (usuarioExistente is not null)
            throw new InvalidOperationException("Ya existe un usuario registrado con ese correo.");

        var rol = await _rolRepository.GetByNombreAsync(request.RolNombre) ?? throw new ArgumentException($"El rol '{request.RolNombre}' no existe.");

        var nuevoUsuario = new Tbl_Usuario
        {
            RolId = rol.RolId,
            Nombre = request.Nombre,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        nuevoUsuario.UsuarioId = await _usuarioRepository.CreateAsync(nuevoUsuario);

        var (token, expiraEn) = _jwtTokenGenerator.GenerateToken(nuevoUsuario.UsuarioId, nuevoUsuario.Email, nuevoUsuario.Nombre, rol.Nombre);

        return new AuthResponseDto
        {
            Token = token,
            Nombre = nuevoUsuario.Nombre,
            Email = nuevoUsuario.Email,
            Rol = rol.Nombre,
            ExpiraEn = expiraEn
        };
    }
}