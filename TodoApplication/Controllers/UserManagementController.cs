using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApplication.Dto;
using TodoApplication.Dto.User;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Controllers;

[Authorize]
public class UserManagementController : Controller
{
    // GET
    private readonly IUserService _userService;
    private readonly IVerificationService _verificationService;
    
    public UserManagementController(
        IUserService  userService,
        IVerificationService verificationService)
    {
        _userService = userService;
        _verificationService = verificationService;
    }
    
    [Authorize(Roles = "SuperAdmin,Manager")]
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

    [HttpGet]
    public async Task<IActionResult> EditUser(Guid id, CancellationToken ct)
    {
        var user = await _userService.GetUserByIdAsync(id, ct);
        if (user == null)
            return NotFound();
        
        var dto = new AdminUpdateUserDto
        {
            email = user.email,
            first_name  = user.first_name,
            last_name   = user.last_name,
            roles = user.roles.Select(r => r.role_Name).ToList()
        };
        
        return View(dto);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(Guid id, AdminUpdateUserDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(dto);
        var result = await _userService.AdminUpdateUserAsync(id, dto, ct);
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

    [HttpPost] 
    public async Task<IActionResult> BlockUnblockUser(Guid id, CancellationToken ct)
    {
        var result = await _userService.BlockUnBlockUser(id, ct);
        
        return RedirectToAction(nameof(UserDetails), new { id = id });
    }
    
    
    [HttpPost] 
    public async Task<IActionResult> UserDelete(Guid id, CancellationToken ct)
    {
        var result = await _userService.UserDeleteAsync(id, ct);
        
        return RedirectToAction(nameof(UserList), new { id = id });
    }
    
    [HttpPost] 
    public async Task<IActionResult> ResentEmailConfirmOTP(Guid id, CancellationToken ct)
    {
        var result = await _userService.ResetSendConfirmEmailOTP(id, ct);
        
        return RedirectToAction(nameof(UserDetails), new { id = id });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManualVerifyUser(ConfirmEmailDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _verificationService.VerifyEmail(dto, ct);
        if (!result.issucceed)
        {
            ModelState.AddModelError("", result.message);
            return View(dto);
        }
        
        return RedirectToAction(nameof(UserDetails));
    }
    

}