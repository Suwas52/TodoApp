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
    private readonly IVerificationService _verificationService;
    private readonly IUnitOfWork _uow;
    public UserService(
        IUsersRepository usersRepository, 
        IRolesRepository rolesRepository, 
        IUserRolesRepository userRolesRepository,
        IVerificationService verificationService,
        IUnitOfWork uow
        )
    {
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _userRolesRepository = userRolesRepository;
        _verificationService = verificationService;
        _uow = uow;
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
            user.updated_at = DateTime.Now;
            
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

    public async Task<Response> ChangePassword(Guid user_id, ChangePasswordDto dto, CancellationToken ct)
    {
        var user = await _usersRepository.GetUserByIdAsync(user_id, ct);
        if (user == null)
            return new Response
            {
                issucceed = false,
                statusCode = 404,
                message = "User not found.",
            };
        
        var correctPassword = PasswordHasher.VerifyHashedPassword(user.password_hash, dto.old_password);

        if (!correctPassword)
            return new Response
            {
                issucceed = false,
                statusCode = 400,
                message = "Old password does not match.",
            };

        if (dto.new_password != dto.confirm_password)
            return new Response
            {
                issucceed = false,
                statusCode = 400,
                message = "Confirm password does not match.",
            };
        
        var newHashPassword = PasswordHasher.HashPassword(dto.new_password);
        user.password_hash = newHashPassword;
        user.password_change_date = DateTime.Now;
        await _uow.SaveChangesAsync(ct);
        return new Response
        {
            issucceed = true,
            statusCode = 200,
            message = "User Updated Successfully.",
        };
    }

    public async Task<Response> UserDeleteAsync(Guid id, CancellationToken ct)
    {
        var user = await _usersRepository.GetUserByIdAsync(id, ct);
        if (user == null)
            return new Response() 
            { issucceed = false, 
                statusCode = 404, 
                message = "User not found." 
            };

        user.is_deleted = true; 
        user.updated_at = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
    
        return new Response()
        {
            issucceed = true,
            statusCode = 200,
            message = "User deleted successfully",
        };
    }

    public async Task<Response> BlockUnBlockUser(Guid id, CancellationToken ct)
    {
        var user = await _usersRepository.GetUserByIdAsync(id, ct);
        if (user == null)
            return new Response() 
                { issucceed = false, 
                    statusCode = 404, 
                    message = "User not found." 
                };

        user.is_blocked = !user.is_blocked; 
        user.updated_at = DateTime.Now;

        await _uow.SaveChangesAsync(ct);
    
        return new Response()
        {
            issucceed = true,
            statusCode = 200,
            message = user.is_blocked ? "User Blocked successfully" : "User Unblocked successfully",
        };
    
    }


    // public async Task<Response> UserActivateInactive(Guid id, CancellationToken ct)
    // {
    //     var user = await _usersRepository.GetUserByIdAsync(id, ct);
    //     if (user == null)
    //         return new Response()
    //         {
    //             issucceed = false, 
    //             statusCode = 404, 
    //             message = "User not found."
    //         };
    //     
    //     user.updated_at = DateTime.Now;
    //
    //     await _uow.SaveChangesAsync(ct);
    //
    //     return new Response()
    //     {
    //         issucceed = true,
    //         statusCode = 200,
    //         message = user.is_active ? "User Active successfully" : "User Inactive successfully",
    //     };
    // }

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

            await _verificationService.SendConfirmEmailOTP(user, ct);
            
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

    public async Task<Response> ResetSendConfirmEmailOTP(Guid user_id, CancellationToken ct)
    {
        var user = await _usersRepository.GetUserByIdAsync(user_id, ct);
        if (user == null)
            return new Response
            {
                issucceed = false,
                statusCode = 409,
                message = "User not found",
            };

        if (user.email_confirmed == true)
            return new Response
            {
                issucceed = false,
                statusCode = 409,
                message = "Email is already Confirmed",
            };
        
        await _verificationService.SendConfirmEmailOTP(user, ct);

        return new Response()
        {
            issucceed = true,
            statusCode = 200,
            message = "Confirmation Email OTP send successfully.",
        };
    }
}