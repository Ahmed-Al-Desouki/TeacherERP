using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration.LinkTeacherToCenter
{
    // مدير النظام يربط مدرساً موجوداً بسنتر (مدرس إضافي في نفس المكان). الربط نشط فوراً بلا موافقة
    public record LinkTeacherToCenterCommand(Guid CenterId, Guid TeacherUserId) : IRequest<Result<CenterTeacherLinkDto>>;

    // جسم الطلب (CenterId يأتي من المسار)
    public record LinkTeacherToCenterRequest(Guid TeacherUserId);
}
