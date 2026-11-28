using FluentValidation;

namespace TeacherERP.Application.Features.Centers.Invitations
{
    public class AcceptCenterInvitationValidator : AbstractValidator<AcceptCenterInvitationCommand>
    {
        public AcceptCenterInvitationValidator()
        {
            RuleFor(x => x.Token).NotEmpty().MaximumLength(100);
        }
    }
}
