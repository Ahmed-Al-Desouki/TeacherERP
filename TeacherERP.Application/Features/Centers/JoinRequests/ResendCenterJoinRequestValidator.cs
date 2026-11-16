using FluentValidation;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class ResendCenterJoinRequestValidator : AbstractValidator<ResendCenterJoinRequestCommand>
    {
        public ResendCenterJoinRequestValidator() => RuleFor(x => x.RequestId).NotEmpty();
    }
}
