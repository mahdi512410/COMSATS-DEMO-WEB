using System.ComponentModel.DataAnnotations;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class Course
{
    public int Id { get; set; }

    [Required, MaxLength(16)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    public int CreditHours { get; set; }

    [MaxLength(120)]
    public string Department { get; set; } = string.Empty;

    public ICollection<CourseOffering> Offerings { get; set; } = new List<CourseOffering>();
}
