using System.ComponentModel.DataAnnotations;

namespace TodoApplication.Dto.User;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; } // The code sent to email

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}