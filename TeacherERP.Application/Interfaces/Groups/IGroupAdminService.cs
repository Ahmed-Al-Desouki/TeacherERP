// جديد — TeacherERP.Application/Interfaces/IGroupAdminService.cs
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    // مصدر واحد لقواعد تعيين وإزالة الطالب المسؤول (يستخدمه التنفيذ الفوري وموافقة المدرس على طلب المساعد)
    public interface IGroupAdminService
    {
        // يعمل في Transaction (Serializable) فيمنع تجاوز الحد بطلبين متزامنين. fulfills: الطلب اللي بيتحقق (يُعتمد في نفس الـ Transaction)
        Task<Result<bool>> ApplyAsync(
            GroupAdminRequestType type,
            Guid groupId,
            Guid studentId,
            Guid actorUserId,
            string? reason,
            GroupAdminRequest? fulfills,
            CancellationToken cancellationToken);
    }
}