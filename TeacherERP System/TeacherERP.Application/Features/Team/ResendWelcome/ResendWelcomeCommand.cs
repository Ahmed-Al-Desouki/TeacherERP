// جديد — TeacherERP.Application/Features/Team/ResendWelcome/ResendWelcomeCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.ResendWelcome
{
    // باسورد مؤقت جديد وإيميل ترحيب لمساعد لسه ما فعّلش حسابه (للمدرس فقط عبر الـ Policy)
    public record ResendWelcomeCommand(Guid UserId) : IRequest<Result<bool>>;
}