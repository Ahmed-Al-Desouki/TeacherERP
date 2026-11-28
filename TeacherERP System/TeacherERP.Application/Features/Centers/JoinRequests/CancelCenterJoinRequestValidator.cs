using FluentValidation;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class CancelCenterJoinRequestValidator : AbstractValidator<CancelCenterJoinRequestCommand>
    {
        public CancelCenterJoinRequestValidator() => RuleFor(x => x.RequestId).NotEmpty();
    }
}
