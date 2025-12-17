namespace TodoApplication.Dto;

public class UserCreateDto
{
    public string email { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string password { get; set; }
}

public class UserUpdateDto : UserCreateDto
{

}