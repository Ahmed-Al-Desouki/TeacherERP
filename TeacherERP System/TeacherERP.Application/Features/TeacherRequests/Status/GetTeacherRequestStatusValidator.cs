// جديد — TeacherERP.Application/Features/TeacherRequests/Status/GetTeacherRequestStatusValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.TeacherRequests.Status
{
    public class GetTeacherRequestStatusValidator : AbstractValidator<GetTeacherRequestStatusQuery>
    {
        public GetTeacherRequestStatusValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.")
                .MaximumLength(100).WithMessage("Token is invalid.");
        }
    }
}
