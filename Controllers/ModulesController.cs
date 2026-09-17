using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace COMSATS.StudentPortal.Web.Controllers;

[Authorize]
public class ModulesController : Controller
{
    public IActionResult ComingSoon(string name)
    {
        ViewData["ModuleName"] = string.IsNullOrWhiteSpace(name) ? "This module" : name;
        return View();
    }
}
