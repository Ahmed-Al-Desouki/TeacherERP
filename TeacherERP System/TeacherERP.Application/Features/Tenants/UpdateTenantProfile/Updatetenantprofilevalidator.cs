using FluentValidation;

namespace TeacherERP.Application.Features.Tenants.UpdateTenantProfile
{
    public class UpdateTenantProfileValidator : AbstractValidator<UpdateTenantProfileCommand>
    {
        public UpdateTenantProfileValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Teacher name is required.")
                .MaximumLength(150).WithMessage("Teacher name must not exceed 150 characters.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(100).WithMessage("Subject must not exceed 100 characters.");
        }
    }
}