using System.ComponentModel.DataAnnotations;
using TodoApplication.Enum;

namespace TodoApplication.Dto;

public class UserCreateDto
{
    [Required]
    [EmailAddress]
    public string email { get; set; }
    [Required]
    public string first_name { get; set; }
    [Required]
    public string last_name { get; set; }
    [Required]
    [MaxLength(8)]
    public string password { get; set; }
    
}

public class UserDetailDto
{
    public Guid user_id { get; set; } 
    public string email { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public bool email_confirmed { get; set; }
    public bool is_deleted { get; set; }
    public bool is_blocked { get; set; }
    public string phone_number { get; set; }
    public string address { get; set; }
    public user_gender gender { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public int login_fail_count { get; set; }
    public DateTime? password_change_date { get; set; }
    public DateTime? last_login_date { get; set; }
    public List<RoleDto> roles { get; set; } 
    
}



public class AdminAddUserDto
{
    public string email { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public List<string> roles { get; set; }
}


public class AdminUpdateUserDto : AdminAddUserDto
{
}

public class UserUpdateDto
{
    [Required, EmailAddress]
    public string email { get; set; }

    [Required, StringLength(50)]
    public string first_name { get; set; }

    [Required, StringLength(50)]
    public string last_name { get; set; }

    [Phone]
    public string phone_number { get; set; }

    [StringLength(250)]
    public string address { get; set; }

    public user_gender gender { get; set; }
}

