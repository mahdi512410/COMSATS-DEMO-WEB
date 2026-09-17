using COMSATS.StudentPortal.Web.Data;
using COMSATS.StudentPortal.Web.Models;
using COMSATS.StudentPortal.Web.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMSATS.StudentPortal.Web.Controllers.Api;

[ApiController, Route("api/faculty"), Authorize(Roles = "Teacher")]
public class FacultyApiController(ApplicationDbContext db, UserManager<ApplicationUser> users) : ControllerBase
{
    private async Task<TeacherProfile?> Me() => await db.TeacherProfiles.SingleOrDefaultAsync(x => x.ApplicationUserId == users.GetUserId(User));
    private async Task<bool> Owns(int offeringId, int teacherId) => await db.CourseOfferings.AnyAsync(x => x.Id == offeringId && x.TeacherProfileId == teacherId);

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var teacher = await Me(); if (teacher is null) return Forbid();
        var today = DateTime.Today.DayOfWeek;
        var offerings = await db.CourseOfferings.Where(x => x.TeacherProfileId == teacher.Id).Include(x => x.Course).Include(x => x.TimetableEntries).ToListAsync();
        var missing = offerings.Sum(x => x.AttendanceSessions.Count(s => s.SessionDate == DateOnly.FromDateTime(DateTime.Today) && !s.IsFinalized));
        return Ok(new { teacher = teacher.User?.FullName, todayClasses = offerings.SelectMany(x => x.TimetableEntries.Where(t => t.DayOfWeek == today).Select(t => new { offeringId = x.Id, code = x.Course!.Code, title = x.Course.Title, start = t.StartTime.ToString(@"hh\:mm"), end = t.EndTime.ToString(@"hh\:mm"), room = x.Room })), unmarkedSessions = missing, offerings = offerings.Select(x => new { x.Id, code = x.Course!.Code, title = x.Course.Title, x.Section }) });
    }

    [HttpGet("offerings")]
    public async Task<IActionResult> Offerings()
    {
        var teacher = await Me(); if (teacher is null) return Forbid();
        return Ok(await db.CourseOfferings.Where(x => x.TeacherProfileId == teacher.Id).Include(x => x.Course).Select(x => new { x.Id, code = x.Course!.Code, title = x.Course.Title, x.Section, x.Room, enrolled = x.Enrollments.Count }).ToListAsync());
    }

    [HttpGet("offerings/{offeringId:int}/roster")]
    public async Task<IActionResult> Roster(int offeringId)
    {
        var teacher = await Me(); if (teacher is null || !await Owns(offeringId, teacher.Id)) return NotFound();
        return Ok(await db.Enrollments.Where(x => x.CourseOfferingId == offeringId).Include(x => x.Student).ThenInclude(x => x!.User).Select(x => new { studentId = x.StudentProfileId, registrationId = x.Student!.RegistrationId, fullName = x.Student.User!.FullName, x.FinalGrade }).ToListAsync());
    }

    [HttpGet("offerings/{offeringId:int}/attendance")]
    public async Task<IActionResult> Attendance(int offeringId, DateOnly date)
    {
        var teacher = await Me(); if (teacher is null || !await Owns(offeringId, teacher.Id)) return NotFound();
        var session = await db.AttendanceSessions.Include(x => x.Records).SingleOrDefaultAsync(x => x.CourseOfferingId == offeringId && x.SessionDate == date);
        return Ok(new { sessionId = session?.Id, finalized = session?.IsFinalized ?? false, records = session?.Records.Select(x => new { x.StudentProfileId, status = x.Status.ToString(), x.Remarks }) ?? [] });
    }

    public record AttendanceSave(DateOnly Date, string SessionType, bool Finalize, List<AttendanceItem> Records);
    public record AttendanceItem(int StudentProfileId, AttendanceStatus Status, string? Remarks);
    [HttpPut("offerings/{offeringId:int}/attendance")]
    public async Task<IActionResult> SaveAttendance(int offeringId, AttendanceSave input)
    {
        var teacher = await Me(); if (teacher is null || !await Owns(offeringId, teacher.Id)) return NotFound();
        var enrolled = await db.Enrollments.Where(x => x.CourseOfferingId == offeringId).Select(x => x.StudentProfileId).ToListAsync();
        if (input.Records.Count != enrolled.Distinct().Count() || input.Records.Any(x => !enrolled.Contains(x.StudentProfileId))) return BadRequest("Mark every student in this offering exactly once.");
        var session = await db.AttendanceSessions.Include(x => x.Records).SingleOrDefaultAsync(x => x.CourseOfferingId == offeringId && x.SessionDate == input.Date);
        if (session?.IsFinalized == true) return BadRequest("Finalized attendance cannot be edited.");
        if (session is null) { session = new AttendanceSession { CourseOfferingId = offeringId, SessionDate = input.Date, SessionType = input.SessionType, MarkedByTeacherProfileId = teacher.Id }; db.AttendanceSessions.Add(session); }
        else db.AttendanceRecords.RemoveRange(session.Records);
        session.IsFinalized = input.Finalize; session.SessionType = input.SessionType;
        foreach (var record in input.Records) db.AttendanceRecords.Add(new AttendanceRecord { Session = session, StudentProfileId = record.StudentProfileId, Status = record.Status, Remarks = record.Remarks });
        await db.SaveChangesAsync(); return Ok(new { session.Id, session.IsFinalized });
    }

    public record ComponentInput(string Name, int MaxMarks, decimal WeightPercent, DateOnly? DueDate);
    [HttpGet("offerings/{offeringId:int}/gradebook")]
    public async Task<IActionResult> Gradebook(int offeringId)
    {
        var teacher = await Me(); if (teacher is null || !await Owns(offeringId, teacher.Id)) return NotFound();
        var components = await db.AssessmentComponents.Where(x => x.CourseOfferingId == offeringId).Include(x => x.Results).ToListAsync();
        return Ok(components.Select(x => new { x.Id, x.Name, x.MaxMarks, x.WeightPercent, x.DueDate, results = x.Results.Select(r => new { r.StudentProfileId, r.MarksObtained }) }));
    }
    [HttpPost("offerings/{offeringId:int}/components")]
    public async Task<IActionResult> AddComponent(int offeringId, ComponentInput input)
    {
        var teacher = await Me(); if (teacher is null || !await Owns(offeringId, teacher.Id)) return NotFound();
        if (input.MaxMarks <= 0 || input.WeightPercent <= 0) return BadRequest("Maximum marks and weight must be positive.");
        var total = await db.AssessmentComponents.Where(x => x.CourseOfferingId == offeringId).SumAsync(x => (decimal?)x.WeightPercent) ?? 0;
        if (total + input.WeightPercent > 100) return BadRequest("Assessment weights cannot exceed 100%.");
        var component = new AssessmentComponent { CourseOfferingId = offeringId, Name = input.Name, MaxMarks = input.MaxMarks, WeightPercent = input.WeightPercent, DueDate = input.DueDate }; db.AssessmentComponents.Add(component); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Gradebook), new { offeringId }, new { component.Id });
    }
    public record MarkInput(int StudentProfileId, int? MarksObtained);
    [HttpPut("components/{componentId:int}/marks")]
    public async Task<IActionResult> SaveMark(int componentId, MarkInput input)
    {
        var teacher = await Me(); var component = await db.AssessmentComponents.Include(x => x.CourseOffering).FirstOrDefaultAsync(x => x.Id == componentId); if (teacher is null || component is null || component.CourseOffering!.TeacherProfileId != teacher.Id) return NotFound();
        if (input.MarksObtained is < 0 || input.MarksObtained > component.MaxMarks || !await db.Enrollments.AnyAsync(x => x.CourseOfferingId == component.CourseOfferingId && x.StudentProfileId == input.StudentProfileId)) return BadRequest("Invalid mark or student.");
        var result = await db.StudentAssessmentResults.SingleOrDefaultAsync(x => x.AssessmentComponentId == componentId && x.StudentProfileId == input.StudentProfileId); if (result is null) { result = new StudentAssessmentResult { AssessmentComponentId = componentId, StudentProfileId = input.StudentProfileId }; db.StudentAssessmentResults.Add(result); } result.MarksObtained = input.MarksObtained; result.GradedAt = input.MarksObtained.HasValue ? DateTime.UtcNow : null; await db.SaveChangesAsync(); return NoContent();
    }
}
