using FluentValidation;

namespace TeacherERP.Application.Features.Centers.Invitations
{
    public class CreateCenterInvitationValidator : AbstractValidator<CreateCenterInvitationCommand>
    {
        public CreateCenterInvitationValidator() => RuleFor(x => x.CenterId).NotEmpty();
    }
}
