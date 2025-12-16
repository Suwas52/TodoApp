namespace TodoApplication.Entities;

public class Roles
{
    public Roles()
    {
        userroles = new HashSet<UserRoles>();
    }
    public Guid role_id { get; set; }
    public string role_name { get; set; }
    public DateTime created_at  { get; set; }
    public DateTime? updated_at  { get; set; }
    public bool is_deleted  { get; set; }

    public ICollection<UserRoles> userroles { get; set; }
}