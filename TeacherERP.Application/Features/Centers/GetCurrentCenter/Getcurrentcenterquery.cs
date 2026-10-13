using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.GetCurrentCenter
{
    // السنتر الذي حدده الطلب الحالي (صريحاً بالـ Header أو تلقائياً لو عضوية واحدة)
    public record GetCurrentCenterQuery : IRequest<Result<CenterDto>>;
}