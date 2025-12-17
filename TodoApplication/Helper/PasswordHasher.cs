
namespace TodoApplication.Helper;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    => BCrypt.Net.BCrypt.HashPassword(password);
    
    public static bool VerifyHashedPassword(string hashedPassword, string password)
    => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}