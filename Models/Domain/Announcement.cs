using System.ComponentModel.DataAnnotations;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class Announcement
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [MaxLength(60)]
    public string Category { get; set; } = string.Empty;

    public DateTime PostedAt { get; set; }

    // Null means visible to every department.
    [MaxLength(120)]
    public string? AudienceDepartment { get; set; }
}
