using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;

namespace SmartTicket.Api.Controllers;

[ApiController]
[Route("api/ml")]
[Authorize(Roles = "Administrador")]
public class MlController : ControllerBase
{
    private readonly IEntrenadorModeloML _entrenador;
    private readonly IWebHostEnvironment _environment;

    public MlController(IEntrenadorModeloML entrenador, IWebHostEnvironment environment)
    {
        _entrenador = entrenador;
        _environment = environment;
    }

    [HttpPost("entrenar")]
    public ActionResult<ResultadoEntrenamiento> Entrenar()
    {
        var rutaCsv = Path.Combine(_environment.ContentRootPath, "Data", "solicitudes_dataset_simulado.csv");

        if (!System.IO.File.Exists(rutaCsv))
            return NotFound(new { message = $"No se encontró el archivo de entrenamiento en: {rutaCsv}" });

        var resultado = _entrenador.EntrenarDesdeCsv(rutaCsv);
        return Ok(resultado);
    }
}
