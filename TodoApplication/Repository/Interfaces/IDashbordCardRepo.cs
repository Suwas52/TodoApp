using TodoApplication.Dto;

namespace TodoApplication.Repository.Interfaces;

public interface IDashbordCardRepo
{
    Task<DashboardCardDto> AdminDashboardCard(CancellationToken ct);
    Task<DashboardCardDto> UserDashboardCard(Guid user_id, CancellationToken ct = default);
}