using FluentValidation;

namespace TeacherERP.Application.Features.Administration.CreateCenter
{
    public class CreateCenterValidator : AbstractValidator<CreateCenterCommand>
    {
        public CreateCenterValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Center name is required.")
                .MinimumLength(2).WithMessage("Center name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Center name must not exceed 100 characters.");

            RuleFor(x => x.OwnerUserId)
                .NotEmpty().WithMessage("Owner teacher is required.");
        }
    }
}
