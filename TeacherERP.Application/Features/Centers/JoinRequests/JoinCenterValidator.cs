using FluentValidation;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class JoinCenterValidator : AbstractValidator<JoinCenterCommand>
    {
        public JoinCenterValidator() => RuleFor(x => x.CenterId).NotEmpty();
    }
}
