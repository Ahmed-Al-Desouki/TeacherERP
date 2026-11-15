using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration.CreateCenter
{
    // مدير النظام ينشئ سنتراً مملوكاً لمدرس موجود، ويصبح هذا المدرس مؤسسه ونشطاً فيه.
    // العنوان والموقع يضبطهما المدرس بعدها من شاشة تعديل السنتر
    public record CreateCenterCommand(string Name, Guid OwnerUserId) : IRequest<Result<AdminCenterDto>>;
}
