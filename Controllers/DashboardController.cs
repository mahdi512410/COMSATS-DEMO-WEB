using COMSATS.StudentPortal.Web.Data;
using COMSATS.StudentPortal.Web.Models;
using COMSATS.StudentPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMSATS.StudentPortal.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private const string CurrentTerm = "Fall 2024";

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var student = await _context.Users.FirstAsync(u => u.Id == userId);

        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.ApplicationUserId == userId && e.Term == CurrentTerm)
            .ToListAsync();

        var gpaHistory = await _context.SemesterResults
            .Where(r => r.ApplicationUserId == userId)
            .OrderBy(r => r.SemesterNumber)
            .ToListAsync();

        var latestResult = gpaHistory.LastOrDefault();

        // Evaluate the clock in application code; SQL Server can then compare
        // the enum column with a simple parameter.
        var today = DateTime.Today.DayOfWeek;
        var timetable = await _context.TimetableEntries
            .Where(t => t.ApplicationUserId == userId && t.DayOfWeek == today)
            .OrderBy(t => t.StartTime)
            .ToListAsync();

        var announcements = await _context.Announcements
            .OrderByDescending(a => a.PostedAt)
            .Take(3)
            .ToListAsync();

        var latestChallan = await _context.FeeChallans
            .Where(f => f.ApplicationUserId == userId)
            .OrderByDescending(f => f.IssuedDate)
            .FirstOrDefaultAsync();

        var gradedAttendance = enrollments.Where(e => e.SessionsTotal > 0).ToList();
        var overallAttendance = gradedAttendance.Count == 0
            ? 0
            : Math.Round(gradedAttendance.Average(e => e.AttendancePercentage), 1);

        var vm = new DashboardViewModel
        {
            Student = student,
            CurrentTerm = CurrentTerm,
            Cgpa = latestResult?.Cgpa ?? 0,
            Sgpa = latestResult?.Sgpa ?? 0,
            OverallAttendance = overallAttendance,
            EnrolledCourseCount = enrollments.Count,
            CreditHoursThisTerm = enrollments.Sum(e => e.Course?.CreditHours ?? 0),
            FeeStatusLabel = latestChallan?.Status.ToString() ?? "No Challan",
            ChallanNumber = latestChallan?.ChallanNumber,
            Courses = enrollments.Select(e => new DashboardViewModel.CourseCard(
                e.Course!.Code,
                e.Course.Title,
                e.InstructorName,
                e.Course.CreditHours,
                e.MidtermMarks,
                e.MidtermTotal,
                e.Grade,
                e.AttendancePercentage,
                e.SessionsAttended,
                e.SessionsTotal,
                e.AttendanceRisk.ToString(),
                e.Status.ToString()
            )).ToList(),
            TodaysTimetable = timetable.Select(t => new DashboardViewModel.TimetableSlot(
                DateTime.Today.Add(t.StartTime).ToString("h:mm tt"),
                DateTime.Today.Add(t.EndTime).ToString("h:mm tt"),
                t.CourseTitle,
                t.Location,
                t.InstructorName
            )).ToList(),
            Announcements = announcements.Select(a => new DashboardViewModel.AnnouncementItem(
                a.Title, a.Body, a.Category, ToRelativeTime(a.PostedAt)
            )).ToList(),
            GpaHistory = gpaHistory.Select(r => new DashboardViewModel.GpaPoint(
                r.SemesterLabel, r.Sgpa, r.Cgpa, r.IsInProgress
            )).ToList()
        };

        return View(vm);
    }

    private static string ToRelativeTime(DateTime postedAtUtc)
    {
        var span = DateTime.UtcNow - postedAtUtc;
        if (span.TotalHours < 24) return $"{Math.Max(1, (int)span.TotalHours)} hrs ago";
        if (span.TotalDays < 2) return "Yesterday";
        return $"{(int)span.TotalDays} days ago";
    }
}
