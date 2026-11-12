// جديد — TeacherERP.Application/Features/Enrollment/GetRequests/GetRequestsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Enrollment.GetRequests
{
    public class GetRequestsValidator : AbstractValidator<GetRequestsQuery>
    {
        public GetRequestsValidator()
        {
            Include(new PagedRequestRules<GetRequestsQuery>());

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Status is invalid.");
        }
    }
}
