// جديد — TeacherERP.Application/Features/Enrollment/RequestGroup/RequestGroupValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Enrollment.RequestGroup
{
    public class RequestGroupValidator : AbstractValidator<RequestGroupCommand>
    {
        public RequestGroupValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Group id is required.");
        }
    }
}