using FluentValidation;
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Finance.CollectCashPayment
{
    public sealed record CollectCashPaymentCommand(Guid StudentId, Guid SessionId, PaymentType PaymentType,
        decimal Amount, PaymentProvider Method = PaymentProvider.Cash)
        : IRequest<Result<CashPaymentResult>>;

    public sealed class CollectCashPaymentCommandValidator : AbstractValidator<CollectCashPaymentCommand>
    {
        public CollectCashPaymentCommandValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.SessionId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).PrecisionScale(10, 2, false);
            RuleFor(x => x.PaymentType).IsInEnum();
            RuleFor(x => x.Method).Must(method =>
                method is PaymentProvider.Cash or PaymentProvider.Other)
                .WithMessage("Select Cash or Other for a manually recorded payment.");
        }
    }
}
