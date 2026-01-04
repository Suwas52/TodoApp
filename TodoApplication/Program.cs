using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using TodoApplication.BackGround_Job;
using TodoApplication.Data;
using TodoApplication.Email;
using TodoApplication.Extension;
using TodoApplication.Helper;
using TodoApplication.Identity;
using TodoApplication.Repository;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Services;
using TodoApplication.Services.Interfaces;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<TodoAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TodoConString"))
    );

builder.Services.AddScoped<ITodoRepository, TodosRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IUserRolesRepository, UserRolesRepository>();

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleSeeder, RoleSeeder>();
builder.Services.AddScoped<IUserSeeder, UserSeeder>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDashbordCardRepo, DashbordCardRepo>();
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISystemInfoFromCookie, SystemInfoFromCookie>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IForgetPasswordMail, ForgetPasswordMailCode>();

builder.Services.AddHangfireServer();

builder.Services.AddHangfire(config =>
{
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(
            builder.Configuration.GetConnectionString("TodoConString")
        );
});

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddAuthentication("TodoApplication")
    .AddCookie("TodoApplication", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(); 

var app = builder.Build();

app.UseRouting();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();


app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new HangfireAuthorizationFilter()
    }
});



await app.SeedApplicationDataAsync();

RecurringJob.AddOrUpdate<TodoExpiredJob>(
    "expired-todo-scanner",
    job => job.ExecuteAsync(),
    Cron.Daily(23, 30),
    TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time")
);

RecurringJob.AddOrUpdate<TodoReminderJob>(
    "todo-daily-reminder",
    job => job.ExecuteAsync(),
    Cron.Daily(1, 0),
    TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time")
);

RecurringJob.AddOrUpdate<TodoSameDayReminderJob>(
    "todo-same-day-1-hour-reminder",
    job => job.ExecuteAsync(),
    "*/5 * * * *" 
);



await app.RunAsync();
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        return httpContext.User.Identity?.IsAuthenticated == true
               && httpContext.User.IsInRole("SuperAdmin");
    }
}
