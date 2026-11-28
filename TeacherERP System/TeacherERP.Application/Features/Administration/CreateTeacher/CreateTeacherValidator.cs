using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Administration.CreateTeacher
{
    public class CreateTeacherValidator : AbstractValidator<CreateTeacherCommand>
    {
        public CreateTeacherValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MinimumLength(2).WithMessage("Full name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

            RuleFor(x => x.TimeZoneId)
                .Must(id => UserTime.IsValid(id) || string.Equals(id, "UTC", StringComparison.OrdinalIgnoreCase))
                .When(x => !string.IsNullOrWhiteSpace(x.TimeZoneId))
                .WithMessage("Time zone must be a valid IANA id such as Africa/Cairo.");
        }
    }
}
