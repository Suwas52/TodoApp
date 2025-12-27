using System.Security.Cryptography;
using System.Text;
using TodoApplication.BackGround_Job;
using TodoApplication.Dto;
using TodoApplication.Dto.User;
using TodoApplication.Entities;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Services;

public class VerificationService : IVerificationService
{
    private const int MaxAttempts = 5;
    private readonly IVerificationCodeRepository _codeRepository;
    private readonly IForgetPasswordMail _forgetPasswordMail;
    private readonly IUnitOfWork _uow;
    private readonly IUsersRepository _usersRepo;
    public VerificationService(
        IVerificationCodeRepository codeRepository,
        IForgetPasswordMail forgetPasswordMail,
        IUsersRepository usersRepo,
        IUnitOfWork uow)
    {
        _codeRepository = codeRepository;
        _forgetPasswordMail = forgetPasswordMail;
        _usersRepo = usersRepo;
        _uow = uow;
    }
    
    public async Task SendConfirmEmailOTP(Users user, CancellationToken ct)
    {
        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var codeHash = SHA256.HashData(Encoding.UTF8.GetBytes(otp));

        var verificationCode = new VerificationCode
        {
            user_id =  user.user_id,
            codehash = codeHash,
            expires_at = DateTime.Now.AddMinutes(10)
        };
        await _codeRepository.AddAsync(verificationCode, ct);

        var dto = new ConfirmEmailMailDto
        {
            Email = user.email,
            Name = user.first_name + " " + user.last_name,
            OtpCode = otp
        };

        await _forgetPasswordMail.SendConfirmationCodeAsync( dto, ct);
    }
    
    public async Task<Response> VerifyEmail(ConfirmEmailDto dto, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
                
            var codeHash = SHA256.HashData(Encoding.UTF8.GetBytes(dto.VerificationCode));
            
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

            user.email_confirmed = true;
            user.updated_at = DateTime.Now;
            await _uow.CommitTransactionAsync(ct);
            return new Response
            {
                issucceed = true,
                statusCode = 200,
                message = "Email Confirm successful",
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