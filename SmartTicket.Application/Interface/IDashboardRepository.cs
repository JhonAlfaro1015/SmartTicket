using SmartTicket.Application.Dtos.Dashboard;

namespace SmartTicket.Application.Interface;

public interface IDashboardRepository
{
    Task<DashboardResumenDto> GetResumenAsync();
}
