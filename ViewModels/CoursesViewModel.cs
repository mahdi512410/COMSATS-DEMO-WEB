namespace COMSATS.StudentPortal.Web.ViewModels;

public class CoursesViewModel
{
    public required string CurrentTerm { get; init; }
    public List<CourseRow> Courses { get; init; } = new();

    public record CourseRow(
        string Code,
        string Title,
        string InstructorName,
        string Location,
        int CreditHours,
        int? MidtermMarks,
        int MidtermTotal,
        string? Grade,
        double AttendancePercentage,
        int SessionsAttended,
        int SessionsTotal,
        string AttendanceRiskLabel,
        string Status);
}
