using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers
{
    public static class CenterErrors
    {
        // نفس الخطأ لسنتر غير موجود أو لا أملكه، حتى لا نكشف وجود سناتر
        public static readonly Error NotFound = new("Center.NotFound", "The center was not found.");
    }
}