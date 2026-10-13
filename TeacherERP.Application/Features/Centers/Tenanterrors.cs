using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers
{
    // أخطاء تحديد السنتر الحالي (مصدر واحد للأكواد والرسائل)
    public static class TenantErrors
    {
        public static readonly Error Required = new("Tenant.Required", "Please select a center to continue.");
        public static readonly Error NoMembership = new("Tenant.NoMembership", "You are not a member of any center yet.");
        public static readonly Error InvalidHeader = new("Tenant.InvalidHeader", "The selected center is invalid.");

        // نفس الخطأ لسنتر غير موجود أو لست عضواً فيه، حتى لا نكشف وجود سناتر
        public static readonly Error Forbidden = new("Tenant.Forbidden", "You do not have access to this center.");
        public static readonly Error Suspended = new("Tenant.Suspended", "This center is currently suspended.");

        private static readonly Error[] All = { Required, NoMembership, InvalidHeader, Forbidden, Suspended };

        public static Error? FromCode(string? code)
            => All.FirstOrDefault(e => string.Equals(e.Code, code, StringComparison.Ordinal));
    }
}