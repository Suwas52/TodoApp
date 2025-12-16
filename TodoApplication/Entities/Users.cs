namespace TodoApplication.Entities;

public class Users
{
    public Users()
    {
        userroles = new HashSet<UserRoles>();
    }
    public Guid user_id { get; set; } 
    public string email { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string password_hash { get; set; }
    public bool email_confirmed { get; set; }
    public bool is_active { get; set; }
    public bool is_deleted { get; set; }
    public bool is_blocked { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public int login_fail_count { get; set; }
    public DateTime? password_change_date { get; set; }
    public DateTime? last_login_date { get; set; }

    public ICollection<UserRoles> userroles { get; set; } 
}