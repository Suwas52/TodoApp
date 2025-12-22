
using TodoApplication.Dto.User;
using TodoApplication.Email;

namespace TodoApplication.BackGround_Job;

public class ForgetPasswordMailCode : IForgetPasswordMail
{
    private readonly IEmailSender _emailSender;
    public ForgetPasswordMailCode(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }
    public async Task SendForgetPasswordAsync(ForgetPasswordMailDto dto, CancellationToken ct)
    {
        var body = $@"
            <p>Hello {dto.Name},</p>
            <p>Your password reset code is:</p>
            <h2>{dto.OtpCode}</h2>
            <p>This code expires in 10 minutes.</p>";

        await _emailSender.SendAsync(
            dto.Email,
            "Reset Your Password",
            body,
            ct: ct
        );
    }
}