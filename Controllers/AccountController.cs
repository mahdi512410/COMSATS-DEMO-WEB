using COMSATS.StudentPortal.Web.Models;
using COMSATS.StudentPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace COMSATS.StudentPortal.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var identifier = model.RegistrationIdOrEmail.Trim();
        var user = identifier.Contains('@')
            ? await _userManager.FindByEmailAsync(identifier)
            : await _userManager.FindByNameAsync(identifier);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "We couldn't find a portal account for that Registration ID or email.");
            return View(model);
        }

        if (!string.Equals(user.CampusName, model.CampusName, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.CampusName), "That account isn't registered under the selected campus.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "This account has been temporarily locked after repeated failed attempts. Contact the IT Helpdesk to reset it.");
            return View(model);
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Incorrect password. Check your CU-PIN and try again.");
            return View(model);
        }

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpGet]
    public IActionResult AccessDenied() => View();
}
