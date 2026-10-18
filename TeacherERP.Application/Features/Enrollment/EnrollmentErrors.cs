// جديد — TeacherERP.Application/Features/Enrollment/EnrollmentErrors.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment
{
    public static class EnrollmentErrors
    {
        // نفس الخطأ لمجموعة غير موجودة أو مؤرشفة أو مدرسها موقوف
        public static readonly Error GroupNotFound = new("Enrollment.GroupNotFound", "The group was not found.");
        public static readonly Error NotAllowed = new("Enrollment.NotAllowed", "You cannot join this teacher.");
        public static readonly Error AlreadyPending = new("Enrollment.AlreadyPending", "You already have a pending request with this teacher.");
        public static readonly Error AlreadyEnrolled = new("Enrollment.AlreadyEnrolled", "The student is already enrolled with this teacher.");
        public static readonly Error NotEnrolled = new("Enrollment.NotEnrolled", "The student is not enrolled with this teacher.");
        public static readonly Error SameGroup = new("Enrollment.SameGroup", "You are already in this group.");
        public static readonly Error GroupFull = new("Enrollment.GroupFull", "The group is full.");
        public static readonly Error RequestNotFound = new("Enrollment.RequestNotFound", "The request was not found.");
        public static readonly Error NotPending = new("Enrollment.NotPending", "The request was already handled.");
    }
}