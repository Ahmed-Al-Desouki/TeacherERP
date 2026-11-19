using FluentValidation;
using MediatR;
using TeacherERP.Application.Finance;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Finance.CreatePaymobIntent;

public sealed record CreatePaymobIntentCommand(Guid SessionId, PaymentType Type, decimal Amount,
    PaymobIntegrationKey Integration, string IdempotencyKey)
    : IRequest<Result<PaymobPaymentIntentResult>>;

public sealed class CreatePaymobIntentCommandValidator : AbstractValidator<CreatePaymobIntentCommand>
{
    public CreatePaymobIntentCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(12, 2, false);
        RuleFor(x => x.Integration).IsInEnum();
        RuleFor(x => x.IdempotencyKey).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
