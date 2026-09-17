using COMSATS.StudentPortal.Web.Models;

namespace COMSATS.StudentPortal.Web.ViewModels;

public class DashboardViewModel
{
    public required ApplicationUser Student { get; init; }
    public required string CurrentTerm { get; init; }

    public decimal Cgpa { get; init; }
    public decimal Sgpa { get; init; }
    public double OverallAttendance { get; init; }
    public int EnrolledCourseCount { get; init; }
    public int CreditHoursThisTerm { get; init; }
    public string FeeStatusLabel { get; init; } = string.Empty;
    public string? ChallanNumber { get; init; }

    public List<CourseCard> Courses { get; init; } = new();
    public List<TimetableSlot> TodaysTimetable { get; init; } = new();
    public List<AnnouncementItem> Announcements { get; init; } = new();
    public List<GpaPoint> GpaHistory { get; init; } = new();

    public record CourseCard(
        string Code,
        string Title,
        string InstructorName,
        int CreditHours,
        int? MidtermMarks,
        int MidtermTotal,
        string? Grade,
        double AttendancePercentage,
        int SessionsAttended,
        int SessionsTotal,
        string AttendanceRiskLabel,
        string Status);

    public record TimetableSlot(string StartTime, string EndTime, string CourseTitle, string Location, string InstructorName);

    public record AnnouncementItem(string Title, string Body, string Category, string RelativeTime);

    public record GpaPoint(string SemesterLabel, decimal Sgpa, decimal Cgpa, bool IsInProgress);
}
