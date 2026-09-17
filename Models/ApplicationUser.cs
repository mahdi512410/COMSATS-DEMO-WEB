using System.ComponentModel.DataAnnotations;
using COMSATS.StudentPortal.Web.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace COMSATS.StudentPortal.Web.Models;

/// <summary>
/// A student account. UserName holds the registration ID (e.g. FA21-BCS-084) which
/// doubles as the sign-in identifier shown on the login screen.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string RegistrationId { get; set; } = string.Empty;

    [MaxLength(120)]
    public string FatherName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? NationalIdMasked { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(10)]
    public string BloodGroup { get; set; } = string.Empty;

    [Required, MaxLength(160)]
    public string ProgramName { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Department { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string CampusName { get; set; } = string.Empty;

    public int CurrentSemester { get; set; }

    [MaxLength(40)]
    public string EnrollmentSession { get; set; } = string.Empty;

    [MaxLength(200)]
    public string CurrentAddress { get; set; } = string.Empty;

    [MaxLength(200)]
    public string PermanentAddress { get; set; } = string.Empty;

    [MaxLength(120)]
    public string PersonalEmail { get; set; } = string.Empty;

    [MaxLength(120)]
    public string EmergencyContactName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string EmergencyContactPhone { get; set; } = string.Empty;

    public int? AdvisorId { get; set; }
    public Advisor? Advisor { get; set; }

    public AcademicStanding AcademicStanding { get; set; } = AcademicStanding.Regular;

    public int TotalCreditsRequired { get; set; } = 134;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<SemesterResult> SemesterResults { get; set; } = new List<SemesterResult>();
    public ICollection<FeeChallan> FeeChallans { get; set; } = new List<FeeChallan>();
    public ICollection<TimetableEntry> TimetableEntries { get; set; } = new List<TimetableEntry>();

    /// <summary>Two-letter initials used to render an avatar without a stock photo.</summary>
    public string Initials
    {
        get
        {
            var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            return parts.Length == 1
                ? parts[0][..1].ToUpperInvariant()
                : $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
        }
    }
}
