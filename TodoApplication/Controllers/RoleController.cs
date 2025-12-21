using Microsoft.AspNetCore.Mvc;

namespace TodoApplication.Controllers;

public class RoleController : Controller
{
    private readonly ILogger<RoleController> _logger;
    
    public RoleController(ILogger<RoleController> logger)
    {
        _logger = logger;
    }
    // GET
    public IActionResult RoleList()
    {
        return View();
    }
    
    
}