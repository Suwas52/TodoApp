using TodoApplication.Helper;

namespace TodoApplication.Extension;

public static class ApplicationSeeder
{
    public static async Task SeedApplicationDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var roleSeeder = services.GetRequiredService<IRoleSeeder>();
            await roleSeeder.SeedAsync();

            var userSeeder = services.GetRequiredService<IUserSeeder>();
            await userSeeder.SeedUserAsync();
            

        }
        catch (Exception ex)
        {
                
            logger.LogError(ex, "An error occurred during role seeding.");
            throw;
        }
    }

}