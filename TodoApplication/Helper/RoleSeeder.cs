using TodoApplication.Constant;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Entities;

namespace TodoApplication.Helper;
public interface IRoleSeeder
{
    Task SeedAsync();
}
public class RoleSeeder : IRoleSeeder
{
    private readonly IRolesRepository _roleRepo;
    private readonly ILogger<RoleSeeder> _logger;
    public RoleSeeder(
        IRolesRepository roleRepo,
        ILogger<RoleSeeder> logger
    )
    {
        _roleRepo = roleRepo;
        _logger = logger;
    }
    public async Task SeedAsync()
    {
        var roles = new[]
        {
            Role.SuperAdmin,
            Role.Manager,
            Role.User,
        };

        foreach (var roleName in roles)
        {
            var roleExist = await _roleRepo.RoleExistsAsync(roleName);
            if (!roleExist)
            {

                var role = new Roles
                {
                    role_name = roleName,
                    is_deleted = false,
                    created_at = DateTime.UtcNow
                };


               await _roleRepo.AddRoleAsync(role);

               _logger.LogInformation($"✅ Role '{roleName}' created.");
            }
        }
    }
}
