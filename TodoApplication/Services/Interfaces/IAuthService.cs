using System.Security.Claims;
using TodoApplication.Dto;
using TodoApplication.Dto.User;

namespace TodoApplication.Services.Interfaces;

public interface IAuthService
{
    Task<Response> RegisterUser(UserCreateDto dto, CancellationToken ct);
    Task<ClaimsPrincipal?> LoginAsync(LoginDto dto, CancellationToken ct);
    Task<UserDetailDto?> UserProfileDetail(CancellationToken ct);
    Task<Response> UpdateUserAsync(Guid user_id, UserUpdateDto dto, CancellationToken ct);
    Task<DashboardCardDto> DashboardCard(CancellationToken ct);
    Task<Response> RequestPasswordResetAsync(string email, CancellationToken ct);
    Task<Response> ResetPassword(ResetPasswordDto dto, CancellationToken ct);
    // Task<Response> ConfirmEmailSendCode(string email, CancellationToken ct);
    Task<Response> VerifyEmail(ConfirmEmailDto dto, CancellationToken ct);
}