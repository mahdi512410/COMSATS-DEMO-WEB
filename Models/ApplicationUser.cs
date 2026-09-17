using System.ComponentModel.DataAnnotations;
using COMSATS.StudentPortal.Web.Models.Domain;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMSATS.StudentPortal.Web.Models;

/// <summary>
/// Identity shared by all portal actors. Academic/personnel details live in a profile.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string PersonalEmail { get; set; } = string.Empty;
    public StudentProfile? StudentProfile { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }

    // Transitional presentation aliases keep the existing student Razor templates compiling;
    // persistence remains exclusively in StudentProfile.
    [NotMapped] public string RegistrationId => StudentProfile?.RegistrationId ?? TeacherProfile?.EmployeeId ?? string.Empty;
    [NotMapped] public string FatherName => StudentProfile?.FatherName ?? string.Empty;
    [NotMapped] public string? NationalIdMasked => StudentProfile?.NationalIdMasked;
    [NotMapped] public DateOnly? DateOfBirth => StudentProfile?.DateOfBirth;
    [NotMapped] public string Gender => StudentProfile?.Gender ?? string.Empty;
    [NotMapped] public string BloodGroup => StudentProfile?.BloodGroup ?? string.Empty;
    [NotMapped] public string ProgramName => StudentProfile?.ProgramName ?? string.Empty;
    [NotMapped] public string Department => StudentProfile?.Department ?? TeacherProfile?.Department ?? string.Empty;
    [NotMapped] public string CampusName => StudentProfile?.CampusName ?? TeacherProfile?.CampusName ?? string.Empty;
    [NotMapped] public int CurrentSemester => StudentProfile?.CurrentSemester ?? 0;
    [NotMapped] public string EnrollmentSession => StudentProfile?.EnrollmentSession ?? string.Empty;
    [NotMapped] public string CurrentAddress => StudentProfile?.CurrentAddress ?? string.Empty;
    [NotMapped] public string PermanentAddress => StudentProfile?.PermanentAddress ?? string.Empty;
    [NotMapped] public string EmergencyContactName => StudentProfile?.EmergencyContactName ?? string.Empty;
    [NotMapped] public string EmergencyContactPhone => StudentProfile?.EmergencyContactPhone ?? string.Empty;
    [NotMapped] public AcademicStanding AcademicStanding => StudentProfile?.AcademicStanding ?? AcademicStanding.Regular;
    [NotMapped] public int TotalCreditsRequired => StudentProfile?.TotalCreditsRequired ?? 0;
    [NotMapped] public TeacherProfile? Advisor => StudentProfile?.AdvisorTeacherProfile;

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
