using COMSATS.StudentPortal.Web.Models;
using COMSATS.StudentPortal.Web.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace COMSATS.StudentPortal.Web.Data;

/// <summary>
/// Seeds one demo student account plus supporting academic records so the portal
/// is browsable immediately after the database is created. Safe to run every
/// startup: every step checks whether the row already exists first.
/// </summary>
public static class DbSeeder
{
    public const string DemoRegistrationId = "FA21-BCS-084";
    public const string DemoPassword = "Comsats@2024";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var advisor = context.Advisors.FirstOrDefault(a => a.Email == "m.haris@comsats.edu.pk");
        if (advisor is null)
        {
            advisor = new Advisor
            {
                FullName = "Dr. Muhammad Haris",
                Title = "Associate Professor, CS Department",
                Department = "Computer Science",
                OfficeLocation = "Cabin 204, Academic Block-I",
                OfficeHours = "Mon, Wed 11:00 AM - 1:00 PM",
                Email = "m.haris@comsats.edu.pk"
            };
            context.Advisors.Add(advisor);
            await context.SaveChangesAsync();
        }

        var courses = new (string Code, string Title, int Credits, string Dept)[]
        {
            ("CSC411", "Machine Learning & Data Mining", 3, "Computer Science"),
            ("CSC471", "Distributed Systems", 3, "Computer Science"),
            ("HUM320", "Technical Report Writing", 3, "Humanities"),
            ("CSC499", "Final Year Project - I", 3, "Computer Science"),
        };

        foreach (var c in courses)
        {
            if (!context.Courses.Any(x => x.Code == c.Code))
            {
                context.Courses.Add(new Course { Code = c.Code, Title = c.Title, CreditHours = c.Credits, Department = c.Dept });
            }
        }
        await context.SaveChangesAsync();

        var existingUser = await userManager.FindByNameAsync(DemoRegistrationId);
        if (existingUser is null)
        {
            var student = new ApplicationUser
            {
                UserName = DemoRegistrationId,
                RegistrationId = DemoRegistrationId,
                Email = "fa21-bcs-084@isb.student.comsats.edu.pk",
                EmailConfirmed = true,
                FullName = "Areeba Fatima",
                FatherName = "Tariq Mahmood Fatima",
                NationalIdMasked = "61101-*******-8",
                DateOfBirth = new DateOnly(2003, 8, 14),
                Gender = "Female",
                BloodGroup = "B+",
                ProgramName = "Bachelor of Science in Computer Science (BSCS)",
                Department = "Computer Science",
                CampusName = "Islamabad Main Campus",
                CurrentSemester = 7,
                EnrollmentSession = "2021 - 2025",
                CurrentAddress = "House 42-B, Street 18, Sector F-8/2, Islamabad, ICT, 44000",
                PermanentAddress = "House 42-B, Street 18, Sector F-8/2, Islamabad, ICT, 44000",
                PersonalEmail = "areeba.fatima.dev@gmail.com",
                // Kept within the original 30-character database column too,
                // so existing installations can seed before their migration runs.
                EmergencyContactName = "Tariq Mahmood (Father)",
                EmergencyContactPhone = "+92 300 987 6543",
                PhoneNumber = "+92 334 512 8890",
                AdvisorId = advisor.Id,
                AcademicStanding = AcademicStanding.DeansHonorRoll,
                TotalCreditsRequired = 134
            };

            var result = await userManager.CreateAsync(student, DemoPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to seed demo student: " + string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            var savedCourses = context.Courses.ToDictionary(c => c.Code);

            context.Enrollments.AddRange(
                new Enrollment
                {
                    ApplicationUserId = student.Id,
                    CourseId = savedCourses["CSC411"].Id,
                    Term = "Fall 2024",
                    InstructorName = "Prof. Dr. Tariq Rahim",
                    Location = "Room CS-204",
                    MidtermMarks = 88,
                    MidtermTotal = 100,
                    Grade = "A",
                    SessionsAttended = 23,
                    SessionsTotal = 25
                },
                new Enrollment
                {
                    ApplicationUserId = student.Id,
                    CourseId = savedCourses["CSC471"].Id,
                    Term = "Fall 2024",
                    InstructorName = "Dr. Usman Khalid",
                    Location = "Lab 3 (CS Dept)",
                    MidtermMarks = 82,
                    MidtermTotal = 100,
                    Grade = "B+",
                    SessionsAttended = 17,
                    SessionsTotal = 20
                },
                new Enrollment
                {
                    ApplicationUserId = student.Id,
                    CourseId = savedCourses["HUM320"].Id,
                    Term = "Fall 2024",
                    InstructorName = "Ms. Sarah Anum",
                    Location = "Room HUM-102",
                    MidtermMarks = 79,
                    MidtermTotal = 100,
                    Grade = "B",
                    SessionsAttended = 14,
                    SessionsTotal = 18
                },
                new Enrollment
                {
                    ApplicationUserId = student.Id,
                    CourseId = savedCourses["CSC499"].Id,
                    Term = "Fall 2024",
                    InstructorName = "Prof. Zafar Iqbal",
                    Location = "AI Lab",
                    MidtermMarks = null,
                    Grade = null,
                    SessionsAttended = 0,
                    SessionsTotal = 0
                }
            );

            context.SemesterResults.AddRange(
                new SemesterResult { ApplicationUserId = student.Id, SemesterNumber = 1, SemesterLabel = "Fall 2021", CreditsEarned = 18, Sgpa = 3.65m, Cgpa = 3.65m, AcademicStanding = AcademicStanding.Regular },
                new SemesterResult { ApplicationUserId = student.Id, SemesterNumber = 2, SemesterLabel = "Spring 2022", CreditsEarned = 19, Sgpa = 3.75m, Cgpa = 3.70m, AcademicStanding = AcademicStanding.DeansHonorRoll },
                new SemesterResult { ApplicationUserId = student.Id, SemesterNumber = 3, SemesterLabel = "Fall 2022", CreditsEarned = 19, Sgpa = 4.00m, Cgpa = 3.80m, AcademicStanding = AcademicStanding.DeansHonorRoll },
                new SemesterResult { ApplicationUserId = student.Id, SemesterNumber = 4, SemesterLabel = "Spring 2023", CreditsEarned = 18, Sgpa = 3.50m, Cgpa = 3.72m, AcademicStanding = AcademicStanding.Regular },
                new SemesterResult { ApplicationUserId = student.Id, SemesterNumber = 5, SemesterLabel = "Fall 2023", CreditsEarned = 19, Sgpa = 3.90m, Cgpa = 3.78m, AcademicStanding = AcademicStanding.DeansHonorRoll },
                new SemesterResult { ApplicationUserId = student.Id, SemesterNumber = 6, SemesterLabel = "Spring 2024", CreditsEarned = 19, Sgpa = 3.88m, Cgpa = 3.81m, AcademicStanding = AcademicStanding.DeansHonorRoll },
                new SemesterResult { ApplicationUserId = student.Id, SemesterNumber = 7, SemesterLabel = "Fall 2024", CreditsEarned = 0, Sgpa = 3.82m, Cgpa = 3.74m, AcademicStanding = AcademicStanding.DeansHonorRoll, IsInProgress = true }
            );

            context.FeeChallans.Add(new FeeChallan
            {
                ApplicationUserId = student.Id,
                ChallanNumber = "49102",
                Term = "Fall 2024",
                AmountDue = 145000m,
                AmountPaid = 145000m,
                IssuedDate = new DateTime(2024, 8, 15),
                DueDate = new DateTime(2024, 9, 5),
                Status = FeeStatus.Cleared
            });

            context.TimetableEntries.AddRange(
                new TimetableEntry { ApplicationUserId = student.Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 30, 0), CourseTitle = "Machine Learning (Lec)", Location = "Room CS-204", InstructorName = "Dr. Tariq Rahim" },
                new TimetableEntry { ApplicationUserId = student.Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 30, 0), CourseTitle = "Distributed Systems Lab", Location = "Lab 3 (CS Dept)", InstructorName = "Engr. Hamza Rauf" },
                new TimetableEntry { ApplicationUserId = student.Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 30, 0), CourseTitle = "Technical Report Writing", Location = "Room HUM-102", InstructorName = "Ms. Sarah Anum" }
            );

            await context.SaveChangesAsync();
        }

        if (!context.Announcements.Any())
        {
            context.Announcements.AddRange(
                new Announcement { Title = "Fall 2024 Terminal Exam Datesheet Released", Body = "Final exam sessions commence on Dec 24, 2024. Verify any roll number slip clashes with the Examination Branch by Dec 18.", Category = "Exam Branch", PostedAt = DateTime.UtcNow.AddHours(-2) },
                new Announcement { Title = "Final Year Project Proposal Defense", Body = "Panel schedule has been published on the portal. Presentation slide submissions close on Friday at 5:00 PM.", Category = "FYP Committee", PostedAt = DateTime.UtcNow.AddDays(-1) },
                new Announcement { Title = "Library Book Due in 2 Days", Body = "\"Pattern Recognition and ML\" (Accession #89304) is due for renewal or physical return.", Category = "Central Library", PostedAt = DateTime.UtcNow.AddDays(-3) }
            );
            await context.SaveChangesAsync();
        }
    }
}
