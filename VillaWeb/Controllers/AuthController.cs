using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VillaUtility;
using VillaWeb.Models;
using VillaWeb.Models.DTO;
using VillaWeb.Services.IServices;

namespace VillaWeb.Controllers;

 public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDTO obj = new();
        return View(obj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDTO obj)
    {
        APIResponse response = await _authService.LoginAsync<APIResponse>(obj);
        if (response != null && response.IsSucces)
        {
            LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt = handler.ReadJwtToken(model.Token);

            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString(StaticDetails.SessionToken, model.Token);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
            return View(obj);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterationRequestDTO obj)
    {
        APIResponse result = await _authService.RegisterAsync<APIResponse>(obj);
        if (result != null && result.IsSucces)
        {
            return RedirectToAction("Login");
        }
        return View();
    }


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Session.SetString(StaticDetails.SessionToken, "");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}

