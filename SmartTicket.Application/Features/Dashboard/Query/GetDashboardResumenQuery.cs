using MediatR;
using SmartTicket.Application.Dtos.Dashboard;

namespace SmartTicket.Application.Features.Dashboard.Query;

public record GetDashboardResumenQuery : IRequest<DashboardResumenDto>;
