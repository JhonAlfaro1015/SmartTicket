using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Interface;

namespace SmartTicket.Api.Controllers;

[ApiController]
[Route("api/catalogos")]
[Authorize]
public class CatalogosController(
    ICategoriaRepository categoriaRepository,
    IPrioridadRepository prioridadRepository,
    IAreaRepository areaRepository,
    IEstadoRepository estadoRepository) : ControllerBase
{
    private readonly ICategoriaRepository _categoriaRepository = categoriaRepository;
    private readonly IPrioridadRepository _prioridadRepository = prioridadRepository;
    private readonly IAreaRepository _areaRepository = areaRepository;
    private readonly IEstadoRepository _estadoRepository = estadoRepository;

    [HttpGet("categorias")]
    public async Task<IActionResult> Categorias() => Ok(await _categoriaRepository.GetAllAsync());

    [HttpGet("prioridades")]
    public async Task<IActionResult> Prioridades() => Ok(await _prioridadRepository.GetAllAsync());

    [HttpGet("areas")]
    public async Task<IActionResult> Areas() => Ok(await _areaRepository.GetAllAsync());

    [HttpGet("estados")]
    public async Task<IActionResult> Estados() => Ok(await _estadoRepository.GetAllAsync());
}
