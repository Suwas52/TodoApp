using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using TodoApplication.Dto;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Controllers;

public class AuthController : Controller
{

    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    public AuthController(
        IAuthService authService,
        IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginData(LoginDto dto, CancellationToken ct)
    {
        var principle = await _authService.LoginAsync(dto, ct);
        
        if(principle == null)
            return Unauthorized("Invalid Credentials");

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
    

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("TodoApplication");
        return RedirectToAction("Login");
    }

   
}