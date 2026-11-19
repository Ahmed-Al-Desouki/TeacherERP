using FluentValidation;
using TeacherERP.Domain.Entities;
namespace TeacherERP.Application.Features.Finance.SubmitInstaPayReceipt;
public sealed class SubmitInstaPayReceiptValidator : AbstractValidator<SubmitInstaPayReceiptCommand>
{
    public SubmitInstaPayReceiptValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(10, 2, false);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(256);
    }
}
