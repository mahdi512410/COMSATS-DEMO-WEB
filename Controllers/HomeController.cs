using Microsoft.AspNetCore.Mvc;

namespace COMSATS.StudentPortal.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return User.Identity?.IsAuthenticated == true
            ? RedirectToAction("Index", "Dashboard")
            : RedirectToAction("Login", "Account");
    }

    public IActionResult Error()
    {
        Response.StatusCode = 500;
        return View();
    }
}
