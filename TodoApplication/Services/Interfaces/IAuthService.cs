using System.Security.Claims;
using TodoApplication.Dto;

namespace TodoApplication.Services.Interfaces;

public interface IAuthService
{
    Task<ClaimsPrincipal?> LoginAsync(LoginDto dto, CancellationToken ct);
    Task<UserDetailDto?> UserProfileDetail(CancellationToken ct);
    Task<Response> UpdateUserAsync(Guid user_id, UserUpdateDto dto, CancellationToken ct);
    Task<DashboardCardDto> DashboardCard(CancellationToken ct);

}