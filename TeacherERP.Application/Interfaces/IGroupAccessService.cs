// جديد — TeacherERP.Application/Interfaces/IGroupAccessService.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    public enum GroupAccessLevel { Staff, GroupAdmin }

    // صلاحية إدارة مجموعة (حضور ومحتوى) للمستخدم الحالي في المدرس الحالي:
    // المدرس والمساعد على كل المجموعات، والطالب المسؤول على مجموعته فقط (وهو غير موقوف)
    public interface IGroupAccessService
    {
        Task<Result<GroupAccessLevel>> EnsureCanManageGroupAsync(Guid groupId, CancellationToken cancellationToken);

        // مجموعة الطالب المسؤول الحالي، أو GroupAdmin.NotAllowed لو غير مسؤول
        Task<Result<Guid>> GetAdminGroupIdAsync(CancellationToken cancellationToken);
    }
}