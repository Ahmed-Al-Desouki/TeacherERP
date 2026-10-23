// جديد — TeacherERP.Application/Features/Attendance/GetDisplayCode/GetDisplayCodeQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetDisplayCode
{
    // الكود المتغير الحالي لعرضه على شاشة السنتر (وضع الكود المتغير فقط)
    public record GetDisplayCodeQuery : IRequest<Result<DisplayCodeDto>>;
}