using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Sessions.LogoutAll
{
    // إنهاء كل جلسات المستخدم على كل أجهزته (بما فيها الحالية)
    public record LogoutAllCommand : IRequest<Result<bool>>;
}