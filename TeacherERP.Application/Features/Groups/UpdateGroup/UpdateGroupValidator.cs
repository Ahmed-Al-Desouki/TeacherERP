// جديد — TeacherERP.Application/Features/Groups/UpdateGroup/UpdateGroupValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Groups.UpdateGroup
{
    public class UpdateGroupValidator : AbstractValidator<UpdateGroupCommand>
    {
        public UpdateGroupValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Group id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Group name is required.")
                .MaximumLength(150).WithMessage("Group name must not exceed 150 characters.");

            RuleFor(x => x.GradeLevel)
                .NotEmpty().WithMessage("Grade level is required.")
                .MaximumLength(50).WithMessage("Grade level must not exceed 50 characters.");

            RuleFor(x => x.CenterId)
                .NotEmpty().WithMessage("Center id is required.");

            RuleFor(x => x.Room)
                .MaximumLength(50).WithMessage("Room must not exceed 50 characters.");

            RuleFor(x => x.Capacity)
                .InclusiveBetween(1, 500).When(x => x.Capacity.HasValue)
                .WithMessage("Capacity must be between 1 and 500.");

            RuleFor(x => x.WhatsAppGroupLink)
                .MaximumLength(500).WithMessage("WhatsApp link must not exceed 500 characters.")
                .Must(BeHttpUrl).WithMessage("WhatsApp link must be a valid http or https URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.WhatsAppGroupLink));
        }

        private static bool BeHttpUrl(string? value)
            => Uri.TryCreate(value, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }
}