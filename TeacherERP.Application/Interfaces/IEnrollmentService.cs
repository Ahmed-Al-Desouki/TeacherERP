// جديد — TeacherERP.Application/Interfaces/IEnrollmentService.cs
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // تفعيل طلب انضمام أو نقل: مصدر واحد للقواعد (يستخدمه القبول التلقائي وموافقة الموظف)
    public interface IEnrollmentService
    {
        // decidedByUserId فارغ لو القبول تلقائي. يحفظ التغييرات داخل Transaction، والطلب لازم يكون متتبَّعاً
        Task<Result<bool>> ApproveAsync(EnrollmentRequest request, Guid? decidedByUserId, CancellationToken cancellationToken);
    }
}