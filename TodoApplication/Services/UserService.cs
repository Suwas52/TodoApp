using Microsoft.EntityFrameworkCore;
using TodoApplication.Dto;
using TodoApplication.Dto.User;
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
    private readonly IUnitOfWork _uow;
    public UserService(
        IUsersRepository usersRepository, 
        IRolesRepository rolesRepository, 
        IUserRolesRepository userRolesRepository,
        IUnitOfWork uow
        )
    {
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _userRolesRepository = userRolesRepository;
        _uow = uow;
    }
    public async Task<Response> RegisterUser(UserCreateDto dto, CancellationToken ct)
    {
        var emailUserExist = await _usersRepository.EmailExistsAsync(dto.email, ct);
        
        if (emailUserExist)
            return new Response
            {
                issucceed = false,
                statusCode = 404,
                message = "Email is already in use.",
            };
        
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
            message = "User Register successfully.",
        };

    }

    public async Task<Response> AdminUpdateUserAsync(Guid id, AdminUpdateUserDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var user = await _usersRepository.GetUserByIdAsync(id, ct);
            if (user == null)
                return new Response
                {
                    issucceed = false,
                    statusCode = 404,
                    message = "User not found.",
                };

            var rolesExist = await _rolesRepository.RolesExistsAsync(dto.roles, ct);
            if (!rolesExist)
                return new Response
                {
                    issucceed = false,
                    statusCode = 404,
                    message = "One or more roles does not found.",
                };
        
            user.first_name = dto.first_name;
            user.last_name = dto.last_name;
            user.email = dto.email;
            
            await RolesAddOrRemove(user,dto.roles, ct);
            await _uow.CommitTransactionAsync(ct);

            return new Response()
            {
                issucceed = true,
                statusCode = 200,
                message = "User Updated Successfully.",
            };
            
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(ct);
            return new Response
            {
                issucceed = false,
                statusCode = 500,
                message = ex.Message,
            };
        }
        
        
    }

    private async Task RolesAddOrRemove(Users user, List<string> roles, CancellationToken ct)
    {
        
        var userroles = await _rolesRepository.GetAllUserRolesAsync(user.user_id, ct);
        
        var roleToAdd = roles.Except(userroles).ToList();
        var roleToRemove = userroles.Except(roles).ToList();
        
        if(roleToAdd.Any())
            await _userRolesRepository.AddToRoles(user, roleToAdd, ct);
        
        if(roleToRemove.Any())
            await _userRolesRepository.RemoveUserFromRolesAsync(user.user_id, roleToRemove, ct);
        
        
    }

    // public async Task<Response> UpdateUserAsync(Guid id, UserUpdateDto dto, CancellationToken ct)
    // {
    //     var user = await _usersRepository.GetUserByIdAsync(id, ct);
    //     if (user == null)
    //         return new Response
    //         {
    //             issucceed = false,
    //             statusCode = 404,
    //             message = "User not found.",
    //         };
    //     
    //     user.first_name = dto.first_name;
    //     user.last_name = dto.last_name;
    //     user.email = dto.email;
    //     user.password_hash = dto.password;
    //     
    // }

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

    public async Task<List<UserListDto>> GetAllUsersAsync(CancellationToken ct)
    {

        var userList = await _usersRepository.GetUsers().ToListAsync(ct);
        return userList;
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        var  user = await _usersRepository.GetUserByIdAsync(id, ct);
        if (user == null)
            return null;
        var mapData = UserMapping.ToDto(user);
        return mapData;
    }

    public async Task<Response> AdminAddUserAsync(AdminAddUserDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var emailUserExist = await _usersRepository.EmailExistsAsync(dto.email, ct);
            if (emailUserExist)
            {
                await _uow.RollbackTransactionAsync(ct);
                return new Response
                {
                    issucceed = false,
                    statusCode = 404,
                    message = "Email is already in use.",
                };
            }
            var passGenerator = PasswordGenerator.GeneratePassword(10);
            var hashPassword = PasswordHasher.HashPassword(passGenerator);

            var user = new Users
            {
                first_name = dto.first_name,
                last_name = dto.last_name,
                email = dto.email,
                is_active = true,
                is_deleted = false,
                password_hash = hashPassword
            };
            await _usersRepository.AddUserAsync(user, ct);
            bool rolesExist = await _rolesRepository.RolesExistsAsync(dto.roles, ct);
            if (!rolesExist)
            {
                await _uow.RollbackTransactionAsync(ct);
                return new Response()
                {
                    issucceed = false,
                    statusCode = StatusCodes.Status404NotFound,
                    message = "One or more Roles not found",
                };
            }

            var roleResult = await _userRolesRepository.AddToRoles(user, dto.roles, ct);
            if (!roleResult.issucceed)
            {
                await _uow.RollbackTransactionAsync(ct);
                return roleResult;
            }
            
            await _uow.CommitTransactionAsync(ct);

            return new Response()
            {
                issucceed = true,
                statusCode = 200,
                message = "User created successfully.",
            };



        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(ct);
            return new Response()
            {
                issucceed = false,
                statusCode = StatusCodes.Status500InternalServerError,
                message = ex.Message,
            };
        }
    }
}