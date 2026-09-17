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

    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<TeacherProfile> TeacherProfiles => Set<TeacherProfile>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseOffering> CourseOfferings => Set<CourseOffering>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<SemesterResult> SemesterResults => Set<SemesterResult>();
    public DbSet<FeeChallan> FeeChallans => Set<FeeChallan>();
    public DbSet<TimetableEntry> TimetableEntries => Set<TimetableEntry>();
    public DbSet<AttendanceSession> AttendanceSessions => Set<AttendanceSession>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<AssessmentComponent> AssessmentComponents => Set<AssessmentComponent>();
    public DbSet<StudentAssessmentResult> StudentAssessmentResults => Set<StudentAssessmentResult>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<StudentProfile>().HasIndex(x => x.ApplicationUserId).IsUnique();
        builder.Entity<StudentProfile>().HasIndex(x => x.RegistrationId).IsUnique();
        builder.Entity<TeacherProfile>().HasIndex(x => x.ApplicationUserId).IsUnique();
        builder.Entity<TeacherProfile>().HasIndex(x => x.EmployeeId).IsUnique();
        builder.Entity<CourseOffering>().HasIndex(x => new { x.CourseId, x.Term, x.Section }).IsUnique();
        builder.Entity<AttendanceRecord>().HasIndex(x => new { x.AttendanceSessionId, x.StudentProfileId }).IsUnique();
        builder.Entity<AssessmentComponent>().Property(x => x.WeightPercent).HasPrecision(5, 2);
        builder.Entity<StudentProfile>().HasOne(x => x.User).WithOne(x => x.StudentProfile).HasForeignKey<StudentProfile>(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<TeacherProfile>().HasOne(x => x.User).WithOne(x => x.TeacherProfile).HasForeignKey<TeacherProfile>(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<StudentProfile>().HasOne(x => x.AdvisorTeacherProfile).WithMany(x => x.Advisees).HasForeignKey(x => x.AdvisorTeacherProfileId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Enrollment>(entity =>
        {
            entity.HasOne(e => e.Student)
                  .WithMany(u => u.Enrollments)
                  .HasForeignKey(e => e.StudentProfileId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CourseOffering)
                  .WithMany(c => c.Enrollments)
                  .HasForeignKey(e => e.CourseOfferingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SemesterResult>()
            .HasOne(r => r.Student)
            .WithMany(u => u.SemesterResults)
            .HasForeignKey(r => r.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FeeChallan>()
            .HasOne(f => f.Student)
            .WithMany(u => u.FeeChallans)
            .HasForeignKey(f => f.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TimetableEntry>()
            .HasOne(t => t.CourseOffering)
            .WithMany(u => u.TimetableEntries)
            .HasForeignKey(t => t.CourseOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CourseOffering>().HasOne(x => x.Course).WithMany(x => x.Offerings).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CourseOffering>().HasOne(x => x.Teacher).WithMany(x => x.Offerings).HasForeignKey(x => x.TeacherProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AttendanceSession>().HasOne(x => x.CourseOffering).WithMany(x => x.AttendanceSessions).HasForeignKey(x => x.CourseOfferingId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<AttendanceRecord>().HasOne(x => x.Session).WithMany(x => x.Records).HasForeignKey(x => x.AttendanceSessionId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<AttendanceRecord>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AssessmentComponent>().HasOne(x => x.CourseOffering).WithMany(x => x.AssessmentComponents).HasForeignKey(x => x.CourseOfferingId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<StudentAssessmentResult>().HasOne(x => x.Component).WithMany(x => x.Results).HasForeignKey(x => x.AssessmentComponentId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<StudentAssessmentResult>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentProfileId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Course>()
            .HasIndex(c => c.Code)
            .IsUnique();
    }
}
