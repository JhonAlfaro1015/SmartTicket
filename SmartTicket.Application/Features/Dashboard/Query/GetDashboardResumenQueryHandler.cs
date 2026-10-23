using MediatR;
using SmartTicket.Application.Common;
using SmartTicket.Application.Dtos.Dashboard;
using SmartTicket.Application.Interface;


namespace SmartTicket.Application.Features.Dashboard.Query;

public class GetDashboardResumenQueryHandler(
    IDashboardRepository dashboardRepository,
    ISolicitudLecturaRepository lecturaRepository) : IRequestHandler<GetDashboardResumenQuery, DashboardResumenDto>
{
    private readonly IDashboardRepository _dashboardRepository = dashboardRepository;
    private readonly ISolicitudLecturaRepository _lecturaRepository = lecturaRepository;

    public async Task<DashboardResumenDto> Handle(GetDashboardResumenQuery request, CancellationToken cancellationToken)
    {
        var resumen = await _dashboardRepository.GetResumenAsync();

        // Pendientes, de la más urgente a la menos urgente; a igual prioridad, la que vence primero
        var todas = await _lecturaRepository.ListarAsync(null, null, null, null);

        resumen.Urgentes = todas
            .Where(s => EstadosSistema.Pendientes.Contains(s.EstadoNombre))
            .OrderBy(s => s.PrioridadNivel)
            .ThenBy(s => s.FechaLimiteSLA ?? DateTime.MaxValue)
            .ThenByDescending(s => s.FechaCreacion)
            .Take(DashboardReglas.CantidadUrgentes)
            .Select(s =>
            {
                s.SlaActivo = true;
                return s;
            })
            .ToList();

        return resumen;
    }
}
