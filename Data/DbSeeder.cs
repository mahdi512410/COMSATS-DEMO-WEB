using COMSATS.StudentPortal.Web.Models;
using COMSATS.StudentPortal.Web.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace COMSATS.StudentPortal.Web.Data;

public static class DbSeeder
{
    public const string DemoRegistrationId = "FA21-BCS-084";
    public const string DemoPassword = "Comsats@2024";
    public const string DemoTeacherEmployeeId = "FAC-CS-014";
    public const string DemoTeacherPassword = "Comsats@2024";
    public const string CurrentTerm = "Fall 2024";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Student", "Teacher" }) if (!await roles.RoleExistsAsync(role)) await roles.CreateAsync(new IdentityRole(role));

        var teacherUser = await users.FindByNameAsync(DemoTeacherEmployeeId);
        if (teacherUser is null)
        {
            teacherUser = new ApplicationUser { UserName = DemoTeacherEmployeeId, Email = "m.haris@comsats.edu.pk", EmailConfirmed = true, FullName = "Dr. Muhammad Haris", PersonalEmail = "m.haris@comsats.edu.pk", PhoneNumber = "+92 300 111 2233" };
            var result = await users.CreateAsync(teacherUser, DemoTeacherPassword);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
            await users.AddToRoleAsync(teacherUser, "Teacher");
        }
        var teacher = await db.TeacherProfiles.FirstOrDefaultAsync(x => x.ApplicationUserId == teacherUser.Id);
        if (teacher is null) { teacher = new TeacherProfile { ApplicationUserId = teacherUser.Id, EmployeeId = DemoTeacherEmployeeId, Designation = "Associate Professor", Department = "Computer Science", CampusName = "Islamabad Main Campus", OfficeLocation = "Cabin 204, Academic Block-I", OfficeHours = "Mon, Wed 11:00 AM - 1:00 PM", Specialization = "Machine Learning", JoiningDate = new DateOnly(2015, 9, 1) }; db.TeacherProfiles.Add(teacher); await db.SaveChangesAsync(); }

        var courses = new[] { ("CSC411", "Machine Learning & Data Mining", 3, "Computer Science", "Room CS-204"), ("CSC471", "Distributed Systems", 3, "Computer Science", "Lab 3 (CS Dept)"), ("HUM320", "Technical Report Writing", 3, "Humanities", "Room HUM-102"), ("CSC499", "Final Year Project - I", 3, "Computer Science", "AI Lab") };
        foreach (var item in courses) if (!await db.Courses.AnyAsync(x => x.Code == item.Item1)) db.Courses.Add(new Course { Code = item.Item1, Title = item.Item2, CreditHours = item.Item3, Department = item.Item4 });
        await db.SaveChangesAsync();
        var offeringByCode = new Dictionary<string, CourseOffering>();
        foreach (var item in courses) { var course = await db.Courses.SingleAsync(x => x.Code == item.Item1); var offering = await db.CourseOfferings.Include(x => x.Course).FirstOrDefaultAsync(x => x.CourseId == course.Id && x.Term == CurrentTerm && x.Section == "A"); if (offering is null) { offering = new CourseOffering { CourseId = course.Id, TeacherProfileId = teacher.Id, Term = CurrentTerm, Section = "A", Room = item.Item5 }; db.CourseOfferings.Add(offering); await db.SaveChangesAsync(); } offeringByCode[item.Item1] = offering; }

        var studentUser = await users.FindByNameAsync(DemoRegistrationId);
        if (studentUser is null)
        {
            studentUser = new ApplicationUser { UserName = DemoRegistrationId, Email = "fa21-bcs-084@isb.student.comsats.edu.pk", EmailConfirmed = true, FullName = "Areeba Fatima", PersonalEmail = "areeba.fatima.dev@gmail.com", PhoneNumber = "+92 334 512 8890" };
            var result = await users.CreateAsync(studentUser, DemoPassword);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
            await users.AddToRoleAsync(studentUser, "Student");
        }
        var student = await db.StudentProfiles.FirstOrDefaultAsync(x => x.ApplicationUserId == studentUser.Id);
        if (student is null) { student = new StudentProfile { ApplicationUserId = studentUser.Id, RegistrationId = DemoRegistrationId, FatherName = "Tariq Mahmood Fatima", NationalIdMasked = "61101-*******-8", DateOfBirth = new DateOnly(2003, 8, 14), Gender = "Female", BloodGroup = "B+", ProgramName = "Bachelor of Science in Computer Science (BSCS)", Department = "Computer Science", CampusName = "Islamabad Main Campus", CurrentSemester = 7, EnrollmentSession = "2021 - 2025", CurrentAddress = "Islamabad, ICT", PermanentAddress = "Islamabad, ICT", EmergencyContactName = "Tariq Mahmood (Father)", EmergencyContactPhone = "+92 300 987 6543", AdvisorTeacherProfileId = teacher.Id, AcademicStanding = AcademicStanding.DeansHonorRoll }; db.StudentProfiles.Add(student); await db.SaveChangesAsync(); }
        if (!await db.Enrollments.AnyAsync(x => x.StudentProfileId == student.Id)) { foreach (var offering in offeringByCode.Values) db.Enrollments.Add(new Enrollment { StudentProfileId = student.Id, CourseOfferingId = offering.Id }); await db.SaveChangesAsync(); }
        if (!await db.TimetableEntries.AnyAsync()) { db.TimetableEntries.AddRange(new TimetableEntry { CourseOfferingId = offeringByCode["CSC411"].Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(10,30,0), SessionType = "Lecture" }, new TimetableEntry { CourseOfferingId = offeringByCode["CSC471"].Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(11,0,0), EndTime = new TimeSpan(12,30,0), SessionType = "Lab" }, new TimetableEntry { CourseOfferingId = offeringByCode["HUM320"].Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(14,0,0), EndTime = new TimeSpan(15,30,0), SessionType = "Lecture" }); }
        if (!await db.SemesterResults.AnyAsync(x => x.StudentProfileId == student.Id)) db.SemesterResults.AddRange(new SemesterResult { StudentProfileId = student.Id, SemesterNumber = 6, SemesterLabel = "Spring 2024", CreditsEarned = 19, Sgpa = 3.88m, Cgpa = 3.81m }, new SemesterResult { StudentProfileId = student.Id, SemesterNumber = 7, SemesterLabel = CurrentTerm, CreditsEarned = 0, Sgpa = 3.82m, Cgpa = 3.74m, IsInProgress = true });
        if (!await db.FeeChallans.AnyAsync(x => x.StudentProfileId == student.Id)) db.FeeChallans.Add(new FeeChallan { StudentProfileId = student.Id, ChallanNumber = "49102", Term = CurrentTerm, AmountDue = 145000m, AmountPaid = 145000m, IssuedDate = new DateTime(2024,8,15), DueDate = new DateTime(2024,9,5), Status = FeeStatus.Cleared });
        if (!await db.AssessmentComponents.AnyAsync()) { var component = new AssessmentComponent { CourseOfferingId = offeringByCode["CSC411"].Id, Name = "Midterm", MaxMarks = 100, WeightPercent = 30, DueDate = new DateOnly(2024,10,15) }; db.AssessmentComponents.Add(component); await db.SaveChangesAsync(); db.StudentAssessmentResults.Add(new StudentAssessmentResult { AssessmentComponentId = component.Id, StudentProfileId = student.Id, MarksObtained = 88, GradedAt = DateTime.UtcNow }); }
        if (!await db.AttendanceSessions.AnyAsync()) { foreach (var offering in offeringByCode.Values) { var session = new AttendanceSession { CourseOfferingId = offering.Id, SessionDate = new DateOnly(2024,10,1), MarkedByTeacherProfileId = teacher.Id, IsFinalized = true }; db.AttendanceSessions.Add(session); await db.SaveChangesAsync(); db.AttendanceRecords.Add(new AttendanceRecord { AttendanceSessionId = session.Id, StudentProfileId = student.Id, Status = AttendanceStatus.Present }); } }
        if (!await db.Announcements.AnyAsync()) db.Announcements.Add(new Announcement { Title = "Fall 2024 Terminal Exam Datesheet Released", Body = "Final exam sessions commence on Dec 24, 2024.", Category = "Exam Branch", PostedAt = DateTime.UtcNow.AddHours(-2) });
        await db.SaveChangesAsync();
    }
}
