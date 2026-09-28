using MediatR;
using SmartTicket.Application.Dtos.Dashboard;
using SmartTicket.Application.Interface;


namespace SmartTicket.Application.Features.Dashboard.Query;

public class GetDashboardResumenQueryHandler(IDashboardRepository dashboardRepository) : IRequestHandler<GetDashboardResumenQuery, DashboardResumenDto>
{
    private readonly IDashboardRepository _dashboardRepository = dashboardRepository;

    public Task<DashboardResumenDto> Handle(GetDashboardResumenQuery request, CancellationToken cancellationToken) => _dashboardRepository.GetResumenAsync();
}
