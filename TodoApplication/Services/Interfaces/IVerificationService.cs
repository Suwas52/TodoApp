using TodoApplication.Dto;
using TodoApplication.Dto.User;
using TodoApplication.Entities;

namespace TodoApplication.Services.Interfaces;

public interface IVerificationService
{
    Task SendConfirmEmailOTP(Users user, CancellationToken ct);
    Task<Response> VerifyEmail(ConfirmEmailDto dto, CancellationToken ct);
}