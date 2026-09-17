using System.ComponentModel.DataAnnotations;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class TimetableEntry
{
    public int Id { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? Student { get; set; }

    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    [Required, MaxLength(160)]
    public string CourseTitle { get; set; } = string.Empty;

    [MaxLength(60)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(120)]
    public string InstructorName { get; set; } = string.Empty;
}
