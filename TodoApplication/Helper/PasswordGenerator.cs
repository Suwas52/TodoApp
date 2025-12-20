namespace TodoApplication.Helper;

public static class PasswordGenerator
{
    public static string GeneratePassword(int length)
    {
        string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()?/";
        
        string password = "";

        for (int i = 0; i < length; i++)
        {
            int randomNum = new Random().Next(0, characters.Length);
            password += characters[randomNum];
            
        }
        
        return password;
    }
}