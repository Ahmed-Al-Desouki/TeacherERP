// جديد — TeacherERP.Application/Features/GroupAdmins/RequestGroupAdminChange/RequestGroupAdminChangeValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.GroupAdmins.RequestGroupAdminChange
{
    public class RequestGroupAdminChangeValidator : AbstractValidator<RequestGroupAdminChangeCommand>
    {
        public RequestGroupAdminChangeValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Group id is required.");

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student id is required.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Request type is invalid.");
        }
    }
}