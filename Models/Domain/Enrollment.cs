using System.ComponentModel.DataAnnotations;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class Enrollment
{
    public int Id { get; set; }

    [Required]
    public int StudentProfileId { get; set; }
    public StudentProfile? Student { get; set; }

    public int CourseOfferingId { get; set; }
    public CourseOffering? CourseOffering { get; set; }
    [MaxLength(4)] public string? FinalGrade { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public Course? Course => CourseOffering?.Course;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public string InstructorName => CourseOffering?.Teacher?.User?.FullName ?? string.Empty;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public string Location => CourseOffering?.Room ?? string.Empty;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public int? MidtermMarks => null;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public int MidtermTotal => 100;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public string? Grade => FinalGrade;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public int SessionsAttended => 0;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public int SessionsTotal => 0;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public double AttendancePercentage => 0;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public AttendanceRisk AttendanceRisk => AttendanceRisk.Critical;

}
