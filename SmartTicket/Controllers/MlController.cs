using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;

namespace SmartTicket.Api.Controllers;

[ApiController]
[Route("api/ml")]
[Authorize(Roles = "Administrador")]
public class MlController(IEntrenadorModeloML entrenador, IWebHostEnvironment environment) : ControllerBase
{
    private readonly IEntrenadorModeloML _entrenador = entrenador;
    private readonly IWebHostEnvironment _environment = environment;

    [HttpPost("entrenar")]
    public ActionResult<ResultadoEntrenamiento> Entrenar()
    {
        var carpetaDatos = Path.Combine(_environment.ContentRootPath, "Data");
        var rutaEntrenamiento = Path.Combine(carpetaDatos, "solicitudes_entrenamiento.csv");
        var rutaValidacion = Path.Combine(carpetaDatos, "solicitudes_validacion.csv");

        if (!System.IO.File.Exists(rutaEntrenamiento))
            return NotFound(new { message = $"No se encontró el archivo de entrenamiento en: {rutaEntrenamiento}" });

        return Ok(_entrenador.EntrenarDesdeCsv(rutaEntrenamiento, rutaValidacion));
    }
}
