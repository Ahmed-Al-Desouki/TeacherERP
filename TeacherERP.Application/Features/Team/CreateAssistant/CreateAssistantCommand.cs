// جديد — TeacherERP.Application/Features/Team/CreateAssistant/CreateAssistantCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.CreateAssistant
{
    // المدرس (Owner) يضيف مساعداً بإيميله: حساب جديد بباسورد مؤقت، أو ربط مساعد موجود بمساحته (للمدرس فقط عبر الـ Policy)
    public record CreateAssistantCommand(string FullName, string Email) : IRequest<Result<AssistantDto>>;
}