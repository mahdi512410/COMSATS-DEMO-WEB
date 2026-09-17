using COMSATS.StudentPortal.Web.Data;
using COMSATS.StudentPortal.Web.Models;
using COMSATS.StudentPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMSATS.StudentPortal.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var student = await _context.Users
            .Include(u => u.Advisor)
            .FirstAsync(u => u.Id == userId);

        var history = await _context.SemesterResults
            .Where(r => r.ApplicationUserId == userId)
            .OrderBy(r => r.SemesterNumber)
            .ToListAsync();

        var completedCredits = history.Where(r => !r.IsInProgress).Sum(r => r.CreditsEarned);
        var completionPct = student.TotalCreditsRequired == 0
            ? 0
            : Math.Round(completedCredits * 100.0 / student.TotalCreditsRequired, 2);

        // Batch ranking derived from CGPA relative to the rest of the same-department cohort.
        // Grouped in memory (not translated to SQL) to keep the query portable across
        // EF Core versions that don't support GroupBy + OrderBy + First translation.
        var departmentResults = await _context.SemesterResults
            .Include(r => r.Student)
            .Where(r => !r.IsInProgress && r.Student!.Department == student.Department)
            .ToListAsync();

        var departmentCgpas = departmentResults
            .GroupBy(r => r.ApplicationUserId)
            .Select(g => g.OrderByDescending(r => r.SemesterNumber).First().Cgpa)
            .ToList();

        var myLatestCgpa = history.LastOrDefault()?.Cgpa ?? 0;
        var batchSize = Math.Max(departmentCgpas.Count, 1);
        var batchPosition = departmentCgpas.Count(c => c > myLatestCgpa) + 1;

        var vm = new ProfileViewModel
        {
            Student = student,
            BatchPosition = batchPosition,
            BatchSize = batchSize,
            CompletedCredits = completedCredits,
            DegreeCompletionPercentage = completionPct,
            SemesterHistory = history.Select(r => new ProfileViewModel.SemesterRow(
                r.SemesterLabel,
                r.CreditsEarned,
                r.IsInProgress ? null : r.Sgpa,
                r.Cgpa,
                r.IsInProgress ? "In Progress" : r.AcademicStanding.ToString(),
                r.IsInProgress
            )).ToList()
        };

        return View(vm);
    }
}
