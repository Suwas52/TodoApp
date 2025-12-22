using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.
using TodoApplication.Dto;
using TodoApplication.Dto.User;
using TodoApplication.Helper;
using TodoApplication.Identity;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IUsersRepository _usersRepo;
    private readonly ISystemInfoFromCookie _cookieInfo;
    private readonly IDashbordCardRepo _dashboard;
    public AuthService(
        IUnitOfWork uow,
        IUsersRepository usersRepo,
        ISystemInfoFromCookie cookieInfo,
        IDashbordCardRepo dashboard)
    {
        _uow = uow;
        _usersRepo = usersRepo;
        _cookieInfo = cookieInfo;
        _dashboard = dashboard;
    }
   
    public async Task<ClaimsPrincipal?> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        var user = await _usersRepo.GetUserByEmailAsync(dto.Email, ct);
        if (user == null) return null;

        bool passwordMatch =
            PasswordHasher.VerifyHashedPassword(user.password_hash, dto.Password);

        if (!passwordMatch) return null;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
            new Claim(ClaimTypes.Email, user.email),
            new Claim("FullName", $"{user.first_name} {user.last_name}")
        };

        foreach (var role in user.userroles.Select(x => x.Role.role_name))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        
        user.last_login_date = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);

        return new ClaimsPrincipal(identity);
    }

    public async Task<UserDetailDto?> UserProfileDetail(CancellationToken ct)
    {
        var userId = _cookieInfo.user_id;

        var user = await _usersRepo.GetUserByIdAsync(userId, ct);
        if (user == null)
            return null;
        var mapData = UserMapping.ToDto(user);
        return mapData;
    }
    public async Task<Response> UpdateUserAsync(Guid user_id, UserUpdateDto dto, CancellationToken ct)
    {
        var user = await _usersRepo.GetUserByIdAsync(user_id, ct);
        if (user == null)
            return new Response
            {
                issucceed = false,
                statusCode = 404,
                message = "User not found.",
            };
        
        user.first_name = dto.first_name;
        user.last_name = dto.last_name;
        user.email = dto.email;
        user.address = dto.address;
        user.gender = dto.gender;
        user.phone_number = dto.phone_number;
        user.updated_at = DateTime.UtcNow;
        
        await _uow.SaveChangesAsync(ct);
        return new Response
        {
            issucceed = true,
            statusCode = 200,
            message = "Profile updated successfully updated.",
        };

    }

    public async Task<DashboardCardDto> DashboardCard(CancellationToken ct)
    {
        if (_cookieInfo.IsSuperAdmin || _cookieInfo.IsManager)
        {
            return await _dashboard.AdminDashboardCard(ct);
        }
        return await _dashboard.UserDashboardCard(_cookieInfo.user_id,ct);
    }

    // public async Task<Response> ForgetPassword(string email, CancellationToken ct)
    // {
    //     var user = await _usersRepo.GetUserByEmailAsync(email, ct);
    //     if (user == null)
    //         return new Response
    //         {
    //             issucceed = false,
    //             statusCode = 400,
    //             message = "User not found.",
    //         };
    //     
    //     
    // }

}