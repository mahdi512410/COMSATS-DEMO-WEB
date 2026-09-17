namespace COMSATS.StudentPortal.Web.Models;

public enum AcademicStanding
{
    Regular,
    DeansHonorRoll,
    Probation,
    Suspended
}

public enum FeeStatus
{
    Cleared,
    Due,
    Overdue
}

public enum EnrollmentStatus
{
    Enrolled,
    Completed,
    Withdrawn
}

public enum AttendanceRisk
{
    Safe,
    AtRisk,
    Critical
}

public enum AttendanceStatus { Present, Absent, Leave, Late }
