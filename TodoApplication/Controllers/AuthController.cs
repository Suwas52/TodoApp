using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using TodoApplication.Dto;
using TodoApplication.Services.Interfaces;

namespace TodoApplication.Controllers;

public class AuthController : Controller
{

    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
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

   
}