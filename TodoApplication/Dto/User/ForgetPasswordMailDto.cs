namespace TodoApplication.Dto.User;

public class ForgetPasswordMailDto
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string OtpCode { get; set; }
}

public class ForgetPassword
{
    public string Email { get; set; }
}