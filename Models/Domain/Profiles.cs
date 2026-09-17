using System.ComponentModel.DataAnnotations;
using COMSATS.StudentPortal.Web.Models;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class StudentProfile
{
    public int Id { get; set; }
    [Required] public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    [Required, MaxLength(20)] public string RegistrationId { get; set; } = string.Empty;
    [MaxLength(120)] public string FatherName { get; set; } = string.Empty;
    [MaxLength(20)] public string? NationalIdMasked { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    [MaxLength(20)] public string Gender { get; set; } = string.Empty;
    [MaxLength(10)] public string BloodGroup { get; set; } = string.Empty;
    [MaxLength(160)] public string ProgramName { get; set; } = string.Empty;
    [MaxLength(120)] public string Department { get; set; } = string.Empty;
    [MaxLength(120)] public string CampusName { get; set; } = string.Empty;
    public int CurrentSemester { get; set; }
    [MaxLength(40)] public string EnrollmentSession { get; set; } = string.Empty;
    [MaxLength(200)] public string CurrentAddress { get; set; } = string.Empty;
    [MaxLength(200)] public string PermanentAddress { get; set; } = string.Empty;
    [MaxLength(120)] public string EmergencyContactName { get; set; } = string.Empty;
    [MaxLength(30)] public string EmergencyContactPhone { get; set; } = string.Empty;
    public int? AdvisorTeacherProfileId { get; set; }
    public TeacherProfile? AdvisorTeacherProfile { get; set; }
    public AcademicStanding AcademicStanding { get; set; } = AcademicStanding.Regular;
    public int TotalCreditsRequired { get; set; } = 134;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<SemesterResult> SemesterResults { get; set; } = new List<SemesterResult>();
    public ICollection<FeeChallan> FeeChallans { get; set; } = new List<FeeChallan>();
}

public class TeacherProfile
{
    public int Id { get; set; }
    [Required] public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    [Required, MaxLength(30)] public string EmployeeId { get; set; } = string.Empty;
    [MaxLength(80)] public string Designation { get; set; } = string.Empty;
    [MaxLength(120)] public string Department { get; set; } = string.Empty;
    [MaxLength(120)] public string CampusName { get; set; } = string.Empty;
    [MaxLength(120)] public string OfficeLocation { get; set; } = string.Empty;
    [MaxLength(160)] public string OfficeHours { get; set; } = string.Empty;
    [MaxLength(160)] public string Specialization { get; set; } = string.Empty;
    public DateOnly? JoiningDate { get; set; }
    public ICollection<CourseOffering> Offerings { get; set; } = new List<CourseOffering>();
    public ICollection<StudentProfile> Advisees { get; set; } = new List<StudentProfile>();
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public string Title => Designation;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public string Email => User?.Email ?? string.Empty;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public string FullName => User?.FullName ?? string.Empty;
}
