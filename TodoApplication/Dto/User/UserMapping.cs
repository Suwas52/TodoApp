using TodoApplication.Entities;

namespace TodoApplication.Dto.User;

internal static class UserMapping
{
    public static UserDetailDto ToDto(this Users user)
    {
        return new UserDetailDto
        {
            user_id = user.user_id,
            email = user.email,
            email_confirmed = user.email_confirmed,
            first_name = user.first_name,
            last_name = user.last_name,
          //  is_active =  user.is_active,
            last_login_date = user.last_login_date,
            is_blocked =  user.is_blocked,
            is_deleted =   user.is_deleted,
            address =  user.address,
            phone_number =  user.phone_number,
            gender =   user.gender,
            created_at = user.created_at,
            updated_at = user.updated_at,
            login_fail_count =  user.login_fail_count,
            password_change_date =  user.password_change_date,
            roles = user.userroles.Select(r=> new RoleDto()
            {
                role_id = r.role_id,
                role_Name = r.Role.role_name
            }).ToList()
        };
    }
}