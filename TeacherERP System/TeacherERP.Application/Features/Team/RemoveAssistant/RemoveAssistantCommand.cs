// جديد — TeacherERP.Application/Features/Team/RemoveAssistant/RemoveAssistantCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.RemoveAssistant
{
    // إزالة المساعد من فريقي (العضوية فقط)، ومسح حسابه لو لم يُفعَّل ولا ينتمي لمدرس آخر (للمدرس فقط عبر الـ Policy)
    public record RemoveAssistantCommand(Guid UserId) : IRequest<Result<bool>>;
}