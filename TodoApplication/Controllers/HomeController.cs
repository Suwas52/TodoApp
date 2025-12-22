using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApplication.Dto;
using TodoApplication.Models;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Controllers;

 [Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAuthService _authService;
    private readonly ITodoService _todoService;
    public HomeController(
        IAuthService authService,
        ILogger<HomeController> logger,
        ITodoService todoService)
    {
        _authService = authService;
        _logger = logger;
        _todoService = todoService;
    }

    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var dashboardCard = await _authService.DashboardCard(ct);
        var recenttodo = await _todoService.RecentTodos(ct);
        var dashboard = new dashboard
        {
            dashboard_card = dashboardCard,
            todo_list = recenttodo
        };
        return View(dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Todo()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}