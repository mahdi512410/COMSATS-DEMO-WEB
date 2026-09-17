using COMSATS.StudentPortal.Web.Data;
using COMSATS.StudentPortal.Web.Models;
using COMSATS.StudentPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMSATS.StudentPortal.Web.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private const string CurrentTerm = "Fall 2024";

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.ApplicationUserId == userId && e.Term == CurrentTerm)
            .ToListAsync();

        var vm = new CoursesViewModel
        {
            CurrentTerm = CurrentTerm,
            Courses = enrollments.Select(e => new CoursesViewModel.CourseRow(
                e.Course!.Code,
                e.Course.Title,
                e.InstructorName,
                e.Location,
                e.Course.CreditHours,
                e.MidtermMarks,
                e.MidtermTotal,
                e.Grade,
                e.AttendancePercentage,
                e.SessionsAttended,
                e.SessionsTotal,
                e.AttendanceRisk.ToString(),
                e.Status.ToString()
            )).ToList()
        };

        return View(vm);
    }
}
