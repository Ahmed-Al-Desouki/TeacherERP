// جديد — TeacherERP.Application/Features/Team/SetFinancialDelegation/SetFinancialDelegationValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Team.SetFinancialDelegation
{
    public class SetFinancialDelegationValidator : AbstractValidator<SetFinancialDelegationCommand>
    {
        public SetFinancialDelegationValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required.");
        }
    }
}