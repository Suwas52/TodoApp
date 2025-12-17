using TodoApplication.Constant;
using TodoApplication.Entities;
using TodoApplication.Repository.Interfaces;

namespace TodoApplication.Helper;

public class UserSeeder : IUserSeeder
{
    private readonly IUsersRepository _usersRepo;
    private readonly IRolesRepository _rolesRepo;
    private readonly IUserRolesRepository _userRolesRepo;
    private readonly ILogger<UserSeeder> _logger;

    public UserSeeder(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IUserRolesRepository userRolesRepository,
        ILogger<UserSeeder> logger)
    {
        _usersRepo = usersRepository;
        _rolesRepo = rolesRepository;
        _userRolesRepo = userRolesRepository;
        _logger = logger;
    }

    public async Task SeedUserAsync()
    {
        string email = "superadmin@gmail.com";
        string password = "SuperAdmin@123";

        if (string.IsNullOrWhiteSpace(email))
            email = "superadmin@gmail.com";

        if (string.IsNullOrWhiteSpace(password))
            password = "SuperAdmin@123";

        var existingUser = await _usersRepo.GetUserByEmailAsync(email);

        if (existingUser == null)
        {
            var user = new Users
            {
                first_name = "Super",
                last_name = "Admin",
                email = email,
                password_hash = PasswordHasher.HashPassword(password),
                created_at = DateTime.UtcNow
            };

            await _usersRepo.AddUserAsync(user);

            _logger.LogInformation("✅ SuperAdmin user created successfully");
            
            if (!await _rolesRepo.RoleExistsAsync(Role.SuperAdmin))
            {
                _logger.LogError("❌ SuperAdmin role not found! Run RoleSeeder first.");
                return;
            }
            
            var result = await _userRolesRepo.AddToRole(existingUser, Role.SuperAdmin);
            if (!result.issucceed)
            {
                _logger.LogError("Failed to add role to user");
            }
            _logger.LogInformation("🔐 SuperAdmin role assigned to user");
        }
        else
        {
            _logger.LogInformation("ℹ SuperAdmin user already exists");
        }

    }
}

public interface IUserSeeder
{
    Task SeedUserAsync();
}