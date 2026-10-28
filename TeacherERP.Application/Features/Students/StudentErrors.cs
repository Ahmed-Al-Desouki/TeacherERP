// جديد — TeacherERP.Application/Features/Students/StudentErrors.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students
{
    public static class StudentErrors
    {
        // نفس الخطأ لطالب غير موجود أو ليس طالبك (أو مساعد/المدرس نفسه)
        public static readonly Error NotFound = new("Student.NotFound", "The student was not found.");
        public static readonly Error NotSuspended = new("Student.NotSuspended", "The student is not suspended.");
        public static readonly Error AlreadyBlacklisted = new("Student.AlreadyBlacklisted", "The student is already blacklisted.");
        public static readonly Error NotBlacklisted = new("Student.NotBlacklisted", "The student is not blacklisted.");
    }
}