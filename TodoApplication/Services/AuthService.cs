using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.
using TodoApplication.Dto;
using TodoApplication.Helper;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IUsersRepository _usersRepo;
    public AuthService(
        IUnitOfWork uow,
        IUsersRepository usersRepo)
    {
        _uow = uow;
        _usersRepo = usersRepo;
    }
   
    public async Task<ClaimsPrincipal?> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        var user = await _usersRepo.GetUserByEmailAsync(dto.Email);
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

        return new ClaimsPrincipal(identity);
    }

}