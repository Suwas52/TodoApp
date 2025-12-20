using System.Security.Claims;
using TodoApplication.Dto;

namespace TodoApplication.Services.Interfaces;

public interface IAuthService
{
    Task<ClaimsPrincipal?> LoginAsync(LoginDto dto, CancellationToken ct);
}