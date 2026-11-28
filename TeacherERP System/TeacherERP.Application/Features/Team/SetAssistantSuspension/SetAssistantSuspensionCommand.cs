// جديد — TeacherERP.Application/Features/Team/SetAssistantSuspension/SetAssistantSuspensionCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.SetAssistantSuspension
{
    // إيقاف مؤقت للمساعد (IsSuspended=true) أو إعادة تفعيله (false)، للمدرس فقط عبر الـ Policy
    public record SetAssistantSuspensionCommand(Guid UserId, bool IsSuspended) : IRequest<Result<bool>>;
}