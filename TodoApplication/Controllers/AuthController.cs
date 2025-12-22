using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApplication.Dto;
using TodoApplication.Dto.User;
using TodoApplication.Identity;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Controllers;


public class AuthController : Controller
{

    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly ISystemInfoFromCookie _cookieInfo;
    public AuthController(
        IAuthService authService,
        IUserService userService,
        ISystemInfoFromCookie cookieInfo)
    {
        _authService = authService;
        _userService = userService;
        _cookieInfo = cookieInfo;
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken ct)
    {
        var principle = await _authService.LoginAsync(dto, ct);
        
        if(principle == null)
        {
            ModelState.AddModelError("", "Invalid Credentials");
            return View(dto);
        }

        await HttpContext.SignInAsync(
            "TodoApplication",
            principle);
        
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View("Register",dto);
        var result = await  _userService.RegisterUser(dto, ct);
        if (!result.issucceed)
        {
            ModelState.AddModelError("", result.message);
            return View(dto);
        }

        return RedirectToAction(nameof(Login));
    }
    

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("TodoApplication");
        return RedirectToAction("Login");
    }
    

    [Authorize]
    public IActionResult PasswordChange()
    {
        var dto = new ChangePasswordDto
        {
            user_id =  _cookieInfo.user_id
        };
        return View(dto);
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PasswordChange(ChangePasswordDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View("PasswordChange",dto);
        var result = await  _userService.ChangePassword(dto.user_id, dto, ct);
        if (!result.issucceed)
        {
            ModelState.AddModelError("", result.message);
            return View(dto);
        }

        return RedirectToAction(nameof(PasswordChange));
    }
    
    [Authorize]
    public async Task<IActionResult> Profile(CancellationToken ct)
    {
        var detail = await _authService.UserProfileDetail(ct);
        return View(detail);
    }
    
    [Authorize]
    public async Task<IActionResult> EditProfile(CancellationToken ct)
    {
        var detail = await _authService.UserProfileDetail(ct);
        var dto = new UserUpdateDto
        {
            first_name = detail.first_name,
            last_name = detail.last_name,
            email = detail.email,
            phone_number = detail.phone_number,
            address = detail.address,
            gender = detail.gender,
        };
        return View(dto);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(UserUpdateDto dto, CancellationToken ct)
    {
        if(!ModelState.IsValid)
            return View("EditProfile",dto);
           
        var userId = _cookieInfo.user_id; 
        var result = await _authService.UpdateUserAsync(userId, dto, ct);
        if(!result.issucceed)
        {
            ModelState.AddModelError("", result.message);
            return View(dto);
        }
        
        return RedirectToAction(nameof(Profile));
    }
    
    
    
    

   
}