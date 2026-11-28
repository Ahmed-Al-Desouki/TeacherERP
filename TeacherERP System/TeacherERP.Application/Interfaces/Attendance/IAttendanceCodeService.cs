// جديد — TeacherERP.Application/Interfaces/IAttendanceCodeService.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    public interface IAttendanceCodeService
    {
        // الكود المتغير الحالي للمدرس وثواني بقائه
        string GenerateDynamicCode(Guid tenantId, string secret, DateTime atUtc);
        int SecondsRemaining(DateTime atUtc);

        // الثابت: يطابق السر. المتغير: كود النافذة الحالية أو السابقة. AdminOnly: دائماً false
        bool IsValid(AttendanceMode mode, Guid tenantId, string secret, string code, DateTime nowUtc);
    }
}