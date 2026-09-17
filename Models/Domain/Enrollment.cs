using System.ComponentModel.DataAnnotations;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class Enrollment
{
    public int Id { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? Student { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    [Required, MaxLength(40)]
    public string Term { get; set; } = string.Empty;

    [MaxLength(120)]
    public string InstructorName { get; set; } = string.Empty;

    [MaxLength(60)]
    public string Location { get; set; } = string.Empty;

    public int? MidtermMarks { get; set; }
    public int MidtermTotal { get; set; } = 100;

    [MaxLength(4)]
    public string? Grade { get; set; }

    public int SessionsAttended { get; set; }
    public int SessionsTotal { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

    // Computed, not stored: attendance percentage derived from session counts.
    public double AttendancePercentage => SessionsTotal == 0
        ? 0
        : Math.Round(SessionsAttended * 100.0 / SessionsTotal, 1);

    public AttendanceRisk AttendanceRisk => AttendancePercentage switch
    {
        >= 85 => AttendanceRisk.Safe,
        >= 80 => AttendanceRisk.AtRisk,
        _ => AttendanceRisk.Critical
    };
}
