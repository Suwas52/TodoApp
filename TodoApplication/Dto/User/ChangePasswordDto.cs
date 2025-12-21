using System.ComponentModel.DataAnnotations;

namespace TodoApplication.Dto.User;

public class ChangePasswordDto
{
    public Guid user_id { get; set; }
    [Required]
    public string old_password { get; set; }
    [Required]
    public string new_password { get; set; }
    [Required]
    public string confirm_password { get; set; }
}