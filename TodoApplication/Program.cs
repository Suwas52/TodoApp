using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Extension;
using TodoApplication.Helper;
using TodoApplication.Identity;
using TodoApplication.Repository;
using TodoApplication.Repository.Interfaces;
using TodoApplication.Services;
using TodoApplication.Services.Interfaces;

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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISystemInfoFromCookie, SystemInfoFromCookie>();


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


await app.SeedApplicationDataAsync();
await app.RunAsync();