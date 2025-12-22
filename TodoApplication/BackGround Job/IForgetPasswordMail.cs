using TodoApplication.Dto.User;

namespace TodoApplication.BackGround_Job;

public interface IForgetPasswordMail
{
    Task SendForgetPasswordAsync(ForgetPasswordMailDto dto, CancellationToken ct);
    Task SendConfirmationCodeAsync(ConfirmEmailMailDto dto, CancellationToken ct);
}