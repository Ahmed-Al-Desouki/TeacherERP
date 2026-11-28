// جديد — TeacherERP.Application/Features/Attendance/GetPendingCompensations/GetPendingCompensationsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Attendance.GetPendingCompensations
{
    public class GetPendingCompensationsValidator : AbstractValidator<GetPendingCompensationsQuery>
    {
        public GetPendingCompensationsValidator()
        {
            Include(new PagedRequestRules<GetPendingCompensationsQuery>());
        }
    }
}
