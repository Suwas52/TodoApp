namespace TodoApplication.Entities;

public class UserRoles
{
    public Guid user_id  { get; set; }
    public Guid role_id  { get; set; }
    public Users User  { get; set; }
    public Roles Role { get; set; }
}