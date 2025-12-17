using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Extension;
using TodoApplication.Helper;
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

var app = builder.Build();

await app.SeedApplicationDataAsync();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }


app.UseRouting();


app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


await app.RunAsync();