// جديد — TeacherERP.Application/Features/Attendance/GetSessionAttendance/GetSessionAttendanceValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Attendance.GetSessionAttendance
{
    public class GetSessionAttendanceValidator : AbstractValidator<GetSessionAttendanceQuery>
    {
        public GetSessionAttendanceValidator()
        {
            Include(new PagedRequestRules<GetSessionAttendanceQuery>());

            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session id is required.");
        }
    }
}
