using TodoApplication.Dto;
using TodoApplication.Entities;

namespace TodoApplication.Repository.Interfaces;

public interface IUserRolesRepository
{
    Task AddUserRolesAsync(UserRoles userRoles, CancellationToken ct);
    Task<Response> AddToRole(Users user, string RoleName, CancellationToken ct = default);
    Task UpdateUserRoleAsync(UserRoles userRoles, CancellationToken ct); 
    Task DeleteUserRolesAsync(UserRoles userRoles, CancellationToken ct);
    // Task<UserRoles?> GetUserRoleByIdAsync(int id, CancellationToken ct);
    // Task<List<UserRoles>> GetAllUserRoleAsync(CancellationToken ct);
}