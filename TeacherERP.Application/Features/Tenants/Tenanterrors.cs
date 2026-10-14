using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Tenants
{
    // أخطاء تحديد مساحة المدرس الحالية (Tenant). الأكواد والرسائل بمفردات الـ API: "Teacher"
    public static class TenantErrors
    {
        public static readonly Error Required = new("Teacher.Required", "Please select a teacher to continue.");
        public static readonly Error NoMembership = new("Teacher.NoMembership", "You have not joined any teacher yet.");
        public static readonly Error InvalidHeader = new("Teacher.InvalidHeader", "The selected teacher is invalid.");

        // نفس الخطأ لمدرس غير موجود أو لست عضواً عنده، حتى لا نكشف وجود مساحات
        public static readonly Error Forbidden = new("Teacher.Forbidden", "You do not have access to this teacher.");
        public static readonly Error Suspended = new("Teacher.Suspended", "This teacher is currently suspended.");

        // عضو عند المدرس لكن دوره لا يسمح بهذا الإجراء
        public static readonly Error InsufficientRole = new("Teacher.InsufficientRole", "Your role does not allow this action.");

        private static readonly Error[] All = { Required, NoMembership, InvalidHeader, Forbidden, Suspended, InsufficientRole };

        public static Error? FromCode(string? code)
            => All.FirstOrDefault(e => string.Equals(e.Code, code, StringComparison.Ordinal));
    }
}