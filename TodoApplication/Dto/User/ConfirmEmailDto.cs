using System.ComponentModel.DataAnnotations;

namespace TodoApplication.Dto.User;

public class ConfirmEmailDto
{
    [Required]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter the 6-digit verification code.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits.")]
    public string VerificationCode { get; set; }
}

public class ConfirmEmailMailDto
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string OtpCode { get; set; }
}