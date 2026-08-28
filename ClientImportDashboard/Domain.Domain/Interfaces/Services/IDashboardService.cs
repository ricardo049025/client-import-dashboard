using Domain.Domain.DTOs.Responses;

namespace Domain.Domain.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetDashboardSummaryAsync();
}
