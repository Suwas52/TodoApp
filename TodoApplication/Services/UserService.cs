using TodoApplication.Dto;
using TodoApplication.Entities;
using TodoApplication.Helper;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Services;

public class UserService : IUserService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IUserRolesRepository _userRolesRepository;
    public UserService(
        IUsersRepository usersRepository, 
        IRolesRepository rolesRepository, 
        IUserRolesRepository userRolesRepository
        )
    {
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _userRolesRepository = userRolesRepository;
    }
    public async Task<Response> CreateUserAsync(UserCreateDto dto, CancellationToken ct)
    {
        
        var user = new Users
        {
            first_name =  dto.first_name,
            last_name =  dto.last_name,
            email =  dto.email,
            password_hash = PasswordHasher.HashPassword(dto.password),
            created_at = DateTime.UtcNow,
            is_active =  true
        };
        
        await _usersRepository.AddUserAsync(user, ct);
        
        var role = await _rolesRepository.GetRoleByNameAsync("User", ct);

        var userRole = new UserRoles
        {
            user_id = user.user_id,
            role_id = role.role_id
        };

        await _userRolesRepository.AddUserRolesAsync(userRole, ct);
        return new Response()
        {
            issucceed = true,
            statusCode = 200,
            message = "User created successfully.",
        };

    }

    public Task<Response> UpdateUserAsync(Guid id, UserUpdateDto dto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UserDeleteAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UserBlockAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UserUnblockAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UserActivateAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<Users>> GetAllUsersAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Users?> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}