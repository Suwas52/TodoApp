using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using TodoApplication.BackGround_Job;
//using Microsoft.AspNetCore.Authentication.
using TodoApplication.Dto;
using TodoApplication.Dto.User;
using TodoApplication.Entities;
using TodoApplication.Helper;
using TodoApplication.Identity;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Services;

public class AuthService : IAuthService
{
    private const int MaxAttempts = 5;
    
    private readonly IUnitOfWork _uow;
    private readonly IUsersRepository _usersRepo;
    private readonly ISystemInfoFromCookie _cookieInfo;
    private readonly IDashbordCardRepo _dashboard;
    private readonly IForgetPasswordMail _forgetPasswordMail;
    private readonly IVerificationCodeRepository _codeRepository;
    public AuthService(
        IUnitOfWork uow,
        IUsersRepository usersRepo,
        ISystemInfoFromCookie cookieInfo,
        IDashbordCardRepo dashboard,
        IForgetPasswordMail forgetPasswordMail,
        IVerificationCodeRepository codeRepository
        )
    {
        _uow = uow;
        _usersRepo = usersRepo;
        _cookieInfo = cookieInfo;
        _dashboard = dashboard;
        _forgetPasswordMail = forgetPasswordMail;
        _codeRepository = codeRepository;
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
    
    public async Task<Response> RequestPasswordResetAsync(string email, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var user = await _usersRepo.GetUserByEmailAsync(email, ct);
            if (user == null)
            {
                await _uow.RollbackTransactionAsync(ct);   
                return new Response
                {
                    issucceed = false,
                    statusCode = 404,
                    message = "User not found.",
                };

            }
                
            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var codeHash = SHA256.HashData(Encoding.UTF8.GetBytes(otp));

             var verificationCode = new VerificationCode
             {
                 user_id =  user.user_id,
                 codehash = codeHash,
                 expires_at = DateTime.UtcNow.AddMinutes(10)
             };
            await _codeRepository.AddAsync(verificationCode, ct);

            var dto = new ForgetPasswordMailDto
            {
                Email = user.email,
                Name = $"{user.first_name} {user.last_name}",
                OtpCode = otp
            };

            await _forgetPasswordMail.SendForgetPasswordAsync( dto, ct);
            await _uow.CommitTransactionAsync(ct);

            return new Response
            {
                issucceed = true,
                statusCode = 200,
                message = "OTP sent to email",
            };
        }catch(Exception e)
        {
            return new Response
            {
                issucceed = false,
                statusCode = 500,
                message = e.Message,
            };
        }
    }
    
    public async Task<Response> ResetPassword(ResetPasswordDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                await _uow.RollbackTransactionAsync(ct);
                return new Response
                {
                    issucceed = true,
                    statusCode = 200,
                    message = "New password and confirm password do not match.",
                };

            }
                
            var codeHash = SHA256.HashData(Encoding.UTF8.GetBytes(dto.Token));
            
            var verification = await _codeRepository.GetValidCodeAsync(codeHash);

            if (verification == null)
            {
                await _uow.RollbackTransactionAsync(ct);
                return new Response
                {
                    issucceed = false,
                    statusCode = 400,
                    message = "Invalid or expired code.",
                };
            }


            if (verification.attempt_count >= MaxAttempts)
            {
                await _uow.RollbackTransactionAsync(ct);
                return new Response
                {
                    issucceed = false,
                    statusCode = 400,
                    message = "Too many attempts.",
                };
            }
    
            var user = await _usersRepo.GetUserByIdAsync(verification.user_id, ct);
            if (user == null)
            {
                await _uow.RollbackTransactionAsync(ct);
                return new Response
                {
                    issucceed = false,
                    statusCode = 400,
                    message = "User not found.",
                };
            }
            
            var hashPassword = PasswordHasher.HashPassword(dto.NewPassword);
    
            user.password_hash = hashPassword;
            user.updated_at = DateTime.UtcNow;
            user.password_change_date = DateTime.UtcNow;

            
            await _uow.CommitTransactionAsync(ct);
            return new Response
            {
                issucceed = true,
                statusCode = 200,
                message = "Password reset successful",
            };
        }
        catch (Exception ex)
        {
            _uow.RollbackTransactionAsync(ct);
            return new Response
            {
                issucceed = false,
                statusCode = 500,
                message = ex.Message,
            };
        }
    }


}