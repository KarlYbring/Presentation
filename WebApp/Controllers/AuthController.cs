using Business.Models;
using Business.Services;
using Data.Entities;
using Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IUserService userService, SignInManager<AppUserEntity> signInManager, IAuthService authService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<AppUserEntity> _signInManager = signInManager;
    private readonly IAuthService _authService = authService;

    [Route("auth/signup")]
    public IActionResult SignUp(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "";

        return View();
    }

    [HttpPost]
    [Route("auth/signup")]
    public async Task<IActionResult> SignUp(SignUpViewModel model, string returnUrl = "~/")
    {

        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ErrorMessage = "";
            return View(model);
        }


        var signUpFormData = model.MapTo<SignUpFormData>();
        var authResult = await _authService.SignUpAsync(signUpFormData);

        if (authResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = authResult.Error;
        return View(model);
    }

    [Route("auth/login")]   
    public IActionResult LogIn(string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    [Route("auth/login")]
    public async Task<IActionResult> LogIn(LoginViewModel model, string returnUrl = "~/")
    {
        if (ModelState.IsValid)
        {
            var signInFormData = model.MapTo<SignInFormData>();
            var authResult = await _authService.SignInAsync(signInFormData);
            
            if (authResult.Succeeded)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userResult = await _userService.GetUserByIdAsync(userId!);
                var user = userResult.Result;

                
                return LocalRedirect(returnUrl);
            }
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = "Unable to login. Try another email or password.";
        return View(model);
    }

    [Route("auth/logout")]
    public async Task<IActionResult> LogOut()
    {
        await _authService.SignOutAsync();
        return LocalRedirect("~/");
    }
}
