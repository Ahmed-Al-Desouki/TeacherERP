// جديد — TeacherERP.Application/Features/Enrollment/RejectRequest/RejectRequestCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.RejectRequest
{
    public record RejectRequestCommand(Guid RequestId, string? Note) : IRequest<Result<bool>>;
}