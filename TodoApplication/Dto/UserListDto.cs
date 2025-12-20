namespace TodoApplication.Dto;

public class UserListDto
{
    public Guid user_id { get; set; } 
    public string email { get; set; }
    public string full_name { get; set; }
    public bool email_confirmed { get; set; }
    public bool is_active { get; set; }
    public bool is_blocked { get; set; }
    
}



