using FluentValidation;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class RejectCenterJoinRequestValidator : AbstractValidator<RejectCenterJoinRequestCommand>
    {
        public RejectCenterJoinRequestValidator()
        {
            RuleFor(x => x.RequestId).NotEmpty();
            RuleFor(x => x.Reason).MaximumLength(300);
        }
    }
}
