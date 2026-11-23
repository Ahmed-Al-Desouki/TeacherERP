using FluentValidation;

namespace TeacherERP.Application.Features.Enrollment.RequestGroupByCode;

public sealed class RequestGroupByCodeValidator : AbstractValidator<RequestGroupByCodeCommand>
{
    public RequestGroupByCodeValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty()
            .MaximumLength(160);
    }
}
