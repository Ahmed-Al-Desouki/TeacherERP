using FluentValidation;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Profile.CompleteProfile
{
    public class CompleteProfileValidator : AbstractValidator<CompleteProfileCommand>
    {
        private const string PhoneMessage = "A valid Egyptian mobile number is required (for example 01012345678).";

        public CompleteProfileValidator(TimeProvider timeProvider)
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Must(BeValidPhone).WithMessage(PhoneMessage);

            RuleFor(x => x.GuardianName)
                .NotEmpty().WithMessage("Guardian name is required.")
                .MinimumLength(2).WithMessage("Guardian name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Guardian name must not exceed 100 characters.");

            RuleFor(x => x.GuardianPhoneNumber)
                .NotEmpty().WithMessage("Guardian phone number is required.")
                .Must(BeValidPhone).WithMessage(PhoneMessage);

            RuleFor(x => x.GuardianRelation)
                .IsInEnum().WithMessage("Guardian relation is invalid.");

            RuleFor(x => x.Gender)
                .IsInEnum().When(x => x.Gender.HasValue).WithMessage("Gender is invalid.");

            RuleFor(x => x.DateOfBirth)
                .Must(date =>
                {
                    var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
                    return date!.Value <= today && date.Value >= today.AddYears(-100);
                })
                .When(x => x.DateOfBirth.HasValue)
                .WithMessage("Date of birth is invalid.");
        }

        private static bool BeValidPhone(string phone) => EgyptianPhoneNumber.TryNormalize(phone, out _);
    }
}