// جديد — TeacherERP.Application/Features/Enrollment/GetMyEnrollments/GetMyEnrollmentsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Enrollment.GetMyEnrollments
{
    public class GetMyEnrollmentsValidator : AbstractValidator<GetMyEnrollmentsQuery>
    {
        public GetMyEnrollmentsValidator()
        {
            Include(new PagedRequestRules<GetMyEnrollmentsQuery>());
        }
    }
}
