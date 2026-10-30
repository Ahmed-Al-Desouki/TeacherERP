// موجود (تعديل) — TeacherERP.Application/Features/Attendance/AttendanceErrors.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance
{
    public static class AttendanceErrors
    {
        // نفس الخطأ لمدرس غير موجود أو موقوف أو كود غلط، حتى لا نكشف شيئاً
        public static readonly Error InvalidCode = new("Attendance.InvalidCode", "The code is invalid or expired.");
        public static readonly Error SelfScanDisabled = new("Attendance.SelfScanDisabled", "This teacher records attendance through the assistant only.");
        public static readonly Error MockLocation = new("Attendance.MockLocation", "Mock locations are not allowed.");
        public static readonly Error NoSessionNow = new("Attendance.NoSessionNow", "There is no session running right now.");
        public static readonly Error ChooseGroup = new("Attendance.ChooseGroup", "More than one session is running. Please choose the group.");
        public static readonly Error Suspended = new("Attendance.Suspended", "Your attendance with this teacher is suspended.");
        public static readonly Error CenterLocationNotSet = new("Attendance.CenterLocationNotSet", "The center location is not set yet.");
        public static readonly Error LocationRequired = new("Attendance.LocationRequired", "Your location is required.");
        public static readonly Error OutOfRange = new("Attendance.OutOfRange", "You are not at the center.");
        public static readonly Error StudentNotFound = new("Attendance.StudentNotFound", "The student card was not found.");
        public static readonly Error NotDynamicMode = new("Attendance.NotDynamicMode", "The attendance mode is not the rotating code.");
        public static readonly Error RecordNotFound = new("Attendance.RecordNotFound", "The attendance record was not found.");
        public static readonly Error NotPending = new("Attendance.NotPending", "The record is not waiting for approval.");

        // الإدخال اليدوي والتصحيح
        public static readonly Error SessionCancelled = new("Attendance.SessionCancelled", "The session is cancelled.");
        public static readonly Error SessionNotStarted = new("Attendance.SessionNotStarted", "The session has not started yet.");
        public static readonly Error ReasonRequired = new("Attendance.ReasonRequired", "A reason is required to correct the attendance of a finished session.");
        public static readonly Error CorrectionWindowClosed = new("Attendance.CorrectionWindowClosed", "The correction period for this session has ended.");
        public static readonly Error StudentNotInGroup = new("Attendance.StudentNotInGroup", "The student was not enrolled in this group at the time of the session.");
        public static readonly Error AlreadyHasRecord = new("Attendance.AlreadyHasRecord", "The student already has a record for this session that cannot be corrected.");
    }
}