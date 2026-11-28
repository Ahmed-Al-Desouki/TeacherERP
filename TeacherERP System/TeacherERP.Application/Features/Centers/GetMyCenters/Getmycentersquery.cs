using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.GetMyCenters
{
    // السناتر التي أملكها أو أدرّس فيها (لا تحتاج مدرس محدد)
    public record GetMyCentersQuery : IRequest<Result<List<CenterDto>>>;
}