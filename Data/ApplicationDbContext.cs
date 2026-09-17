using COMSATS.StudentPortal.Web.Models;
using COMSATS.StudentPortal.Web.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace COMSATS.StudentPortal.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Advisor> Advisors => Set<Advisor>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<SemesterResult> SemesterResults => Set<SemesterResult>();
    public DbSet<FeeChallan> FeeChallans => Set<FeeChallan>();
    public DbSet<TimetableEntry> TimetableEntries => Set<TimetableEntry>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(u => u.RegistrationId).IsUnique();

            entity.HasOne(u => u.Advisor)
                  .WithMany(a => a.Advisees)
                  .HasForeignKey(u => u.AdvisorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Enrollment>(entity =>
        {
            entity.HasOne(e => e.Student)
                  .WithMany(u => u.Enrollments)
                  .HasForeignKey(e => e.ApplicationUserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Course)
                  .WithMany(c => c.Enrollments)
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SemesterResult>()
            .HasOne(r => r.Student)
            .WithMany(u => u.SemesterResults)
            .HasForeignKey(r => r.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FeeChallan>()
            .HasOne(f => f.Student)
            .WithMany(u => u.FeeChallans)
            .HasForeignKey(f => f.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TimetableEntry>()
            .HasOne(t => t.Student)
            .WithMany(u => u.TimetableEntries)
            .HasForeignKey(t => t.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Course>()
            .HasIndex(c => c.Code)
            .IsUnique();
    }
}
