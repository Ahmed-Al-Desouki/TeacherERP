using FluentValidation;

namespace TeacherERP.Application.Features.Administration.LinkTeacherToCenter
{
    public class LinkTeacherToCenterValidator : AbstractValidator<LinkTeacherToCenterCommand>
    {
        public LinkTeacherToCenterValidator()
        {
            RuleFor(x => x.CenterId).NotEmpty().WithMessage("Center is required.");
            RuleFor(x => x.TeacherUserId).NotEmpty().WithMessage("Teacher is required.");
        }
    }
}
