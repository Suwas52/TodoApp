using Microsoft.AspNetCore.Mvc;
using TodoApplication.Dto;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Controllers;

public class UserManagementController : Controller
{
    // GET
    private readonly IUserService _userService;
    
    public UserManagementController(IUserService  userService)
    {
        _userService = userService;
    }
    public IActionResult Index()
    {
        return View();
    }
    
    
    public async Task<IActionResult> UserList(CancellationToken ct)
    {
        var userlist = await _userService.GetAllUsersAsync(ct);
        return View(userlist);
    }

    public IActionResult Create()
    {
        // ViewBag.Roles = new List<string>
        // {
        //     "SuperAdmin",
        //     "Manager",
        //     "User",
        // };
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminAddUserDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View("Create",dto);
        var result = await  _userService.AdminAddUserAsync(dto, ct);
        if (!result.issucceed)
        {
            ModelState.AddModelError("", result.message);
            return View(dto);
        }

        return RedirectToAction(nameof(UserList));
    }

    public async Task<IActionResult> UserDetails(Guid id, CancellationToken ct)
    {
        var userDetail = await _userService.GetUserByIdAsync(id, ct);
        return View(userDetail);
    }
    

}