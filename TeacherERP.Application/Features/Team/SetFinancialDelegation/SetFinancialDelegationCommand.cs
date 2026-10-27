// جديد — TeacherERP.Application/Features/Team/SetFinancialDelegation/SetFinancialDelegationCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.SetFinancialDelegation
{
    // منح أو سحب الصلاحية المالية للمساعد (للمدرس فقط عبر الـ Policy). الفرض الفعلي يتبني مع المالية
    public record SetFinancialDelegationCommand(Guid UserId, bool Granted) : IRequest<Result<bool>>;
}