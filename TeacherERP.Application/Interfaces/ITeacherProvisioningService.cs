using TeacherERP.Application.Features.Administration;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // إنشاء مدرس كامل (حساب بباسورد مؤقت + مساحته + عضوية Owner + إعدادات الحضور) كوحدة واحدة.
    // مصدر واحد يستخدمه مدير النظام الآن، ومسار تسجيل المدرس بالـ QR لاحقاً
    public interface ITeacherProvisioningService
    {
        Task<Result<CreatedTeacherDto>> ProvisionAsync(
            string fullName, string email, string timeZoneId, CancellationToken cancellationToken);
    }
}
