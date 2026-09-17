using System.ComponentModel.DataAnnotations;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class TimetableEntry
{
    public int Id { get; set; }

    public int CourseOfferingId { get; set; }
    public CourseOffering? CourseOffering { get; set; }

    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    [Required, MaxLength(30)] public string SessionType { get; set; } = "Lecture";
}
