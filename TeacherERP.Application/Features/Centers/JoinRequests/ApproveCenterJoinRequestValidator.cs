using FluentValidation;

namespace TeacherERP.Application.Features.Centers.JoinRequests
{
    public class ApproveCenterJoinRequestValidator : AbstractValidator<ApproveCenterJoinRequestCommand>
    {
        public ApproveCenterJoinRequestValidator() => RuleFor(x => x.RequestId).NotEmpty();
    }
}
