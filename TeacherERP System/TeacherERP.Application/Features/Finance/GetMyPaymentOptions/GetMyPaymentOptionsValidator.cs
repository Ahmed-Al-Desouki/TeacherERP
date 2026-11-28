using FluentValidation;
namespace TeacherERP.Application.Features.Finance.GetMyPaymentOptions;
public sealed class GetMyPaymentOptionsValidator : AbstractValidator<GetMyPaymentOptionsQuery>
{ public GetMyPaymentOptionsValidator() => RuleFor(x => x.SessionId).NotEmpty(); }
