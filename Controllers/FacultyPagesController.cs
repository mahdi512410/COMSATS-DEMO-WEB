using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace COMSATS.StudentPortal.Web.Controllers;

[Authorize(Roles = "Teacher")]
[Route("Faculty")]
public class FacultyPagesController : Controller
{
    [HttpGet("Dashboard")] public IActionResult Dashboard() => View("~/Views/Faculty/Dashboard/Index.cshtml");
    [HttpGet("Classes")] public IActionResult Classes() => View("~/Views/Faculty/Classes/Index.cshtml");
    [HttpGet("Attendance")] public IActionResult Attendance() => View("~/Views/Faculty/Attendance/Index.cshtml");
    [HttpGet("Gradebook")] public IActionResult Gradebook() => View("~/Views/Faculty/Gradebook/Index.cshtml");
    [HttpGet("Timetable")] public IActionResult Timetable() => View("~/Views/Faculty/Timetable/Index.cshtml");
    [HttpGet("Profile")] public IActionResult Profile() => View("~/Views/Faculty/Profile/Index.cshtml");
}
