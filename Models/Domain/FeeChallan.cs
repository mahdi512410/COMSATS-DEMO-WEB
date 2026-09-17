using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class FeeChallan
{
    public int Id { get; set; }

    [Required]
    public int StudentProfileId { get; set; }
    public StudentProfile? Student { get; set; }

    [Required, MaxLength(20)]
    public string ChallanNumber { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Term { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal AmountDue { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal AmountPaid { get; set; }

    public DateTime IssuedDate { get; set; }
    public DateTime DueDate { get; set; }

    public FeeStatus Status { get; set; } = FeeStatus.Due;
}
