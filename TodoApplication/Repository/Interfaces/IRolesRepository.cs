using TodoApplication.Entities;

namespace TodoApplication.Repository.Interfaces;

public interface IRolesRepository
{
    Task AddRoleAsync(Roles users);
    Task UpdateRoleAsync(Roles users, CancellationToken ct); 
    Task DeleteRoleAsync(Roles users, CancellationToken ct);
    Task<Roles?> GetRoleByIdAsync(Guid id, CancellationToken ct);
    Task<Roles?> GetRoleByNameAsync(string roleName, CancellationToken ct);
    Task<List<Roles>> GetAllRoleAsync(CancellationToken ct);
    Task<bool> RoleExistsAsync(string roleName);
    Task<bool> RolesExistsAsync(List<string> roleNames, CancellationToken ct);
    Task<List<string>> GetAllUserRolesAsync(Guid user_id, CancellationToken ct);
}