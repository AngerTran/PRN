using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using CapstoneReviewTool.Entities;
using CapstoneReviewTool.ViewModels;

namespace CapstoneReviewTool.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToRoleDashboard(user.Role);
                }
            }
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
        }
        
        return View(model);
    }

    [HttpGet]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");
        
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Lỗi từ nhà cung cấp: {remoteError}");
            return View("Login");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        
        if (signInResult.Succeeded)
        {
            var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (existingUser != null)
            {
                return RedirectToRoleDashboard(existingUser.Role);
            }
            return LocalRedirect(returnUrl);
        }

        // User does not exist, create new user
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
        
        if (email == null)
        {
            ModelState.AddModelError(string.Empty, "Không thể lấy email từ tài khoản.");
            return View("Login");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = name ?? email,
            Role = DetermineRoleFromEmail(email)
        };

        var createResult = await _userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToRoleDashboard(user.Role);
        }

        foreach (var error in createResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View("Login");
    }

    [HttpGet]
    public async Task<IActionResult> DemoLogin(string role)
    {
        var userRole = role.ToLower() switch
        {
            "student" => UserRole.Student,
            "lecturer" => UserRole.Lecturer,
            "admin" => UserRole.Admin,
            _ => UserRole.Student
        };

        var email = $"demo.{role.ToLower()}@fpt.edu.vn";
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = $"Demo {role}",
                Role = userRole
            };
            await _userManager.CreateAsync(user, "Demo123!");
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToRoleDashboard(userRole);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private UserRole DetermineRoleFromEmail(string email)
    {
        if (email.EndsWith("@fpt.edu.vn") && email.Contains("se") || email.Any(char.IsDigit))
        {
            return UserRole.Student;
        }
        if (email.EndsWith("@fe.edu.vn") || email.EndsWith("@fpt.edu.vn"))
        {
            return UserRole.Lecturer;
        }
        return UserRole.Student;
    }

    private IActionResult RedirectToRoleDashboard(UserRole role)
    {
        return role switch
        {
            UserRole.Student => RedirectToAction("Dashboard", "Student"),
            UserRole.Lecturer => RedirectToAction("Dashboard", "Committee"),
            UserRole.Admin => RedirectToAction("Dashboard", "Admin"),
            _ => RedirectToAction("Index", "Home")
        };
    }
}
