using TodoApplication.Dto;
using TodoApplication.Entities;

namespace TodoApplication.Services.Interfaces;

public interface IUserService
{
    Task<Response> CreateUserAsync(UserCreateDto dto, CancellationToken ct);
    Task<Response> UpdateUserAsync(Guid id, UserUpdateDto dto, CancellationToken ct);
    Task<Response> UserDeleteAsync(Guid id,  CancellationToken ct);
    Task<Response> UserBlockAsync(Guid id, CancellationToken ct);
    Task<Response> UserUnblockAsync(Guid id, CancellationToken ct);
    Task<Response> UserActivateAsync(Guid id, CancellationToken ct);
    Task<List<Users>>  GetAllUsersAsync(CancellationToken ct);
    Task<Users?> GetUserByIdAsync(Guid id, CancellationToken ct);
}