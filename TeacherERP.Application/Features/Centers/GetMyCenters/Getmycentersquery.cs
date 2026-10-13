using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.GetMyCenters
{
    // كل السناتر التي المستخدم عضو فيها (لا تحتاج سنتر محدد)
    public record GetMyCentersQuery : IRequest<Result<List<CenterDto>>>;
}