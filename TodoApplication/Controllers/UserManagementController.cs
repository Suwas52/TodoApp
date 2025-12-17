using Microsoft.AspNetCore.Mvc;

namespace TodoApplication.Controllers;

public class UserManagementController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}