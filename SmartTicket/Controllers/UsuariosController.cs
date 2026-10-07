using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Interface;

namespace SmartTicket.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador,Supervisor")]
public class UsuariosController(IUsuarioRepository usuarioRepository) : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

    [HttpGet]
    public async Task<IActionResult> GetByRol([FromQuery] string rol)
    {
        var usuarios = await _usuarioRepository.GetByRolNombreAsync(rol);

        // Proyección explícita: nunca se expone PasswordHash
        var resultado = usuarios.Select(u => new { u.UsuarioId, u.Nombre, u.Email });

        return Ok(resultado);
    }
}
