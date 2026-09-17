using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class SemesterResult
{
    public int Id { get; set; }

    [Required]
    public int StudentProfileId { get; set; }
    public StudentProfile? Student { get; set; }

    public int SemesterNumber { get; set; }

    [Required, MaxLength(40)]
    public string SemesterLabel { get; set; } = string.Empty;

    public int CreditsEarned { get; set; }

    [Column(TypeName = "decimal(3,2)")]
    public decimal Sgpa { get; set; }

    [Column(TypeName = "decimal(3,2)")]
    public decimal Cgpa { get; set; }

    public AcademicStanding AcademicStanding { get; set; } = AcademicStanding.Regular;

    public bool IsInProgress { get; set; }
}
