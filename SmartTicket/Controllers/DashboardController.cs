using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTicket.Application.Dtos.Dashboard;
using SmartTicket.Application.Features.Dashboard.Query;

namespace SmartTicket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Supervisor")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("resumen")]
    public async Task<ActionResult<DashboardResumenDto>> Resumen()
    {
        var result = await _mediator.Send(new GetDashboardResumenQuery());
        return Ok(result);
    }
}
