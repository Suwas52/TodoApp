using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
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
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();



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


app.Run();