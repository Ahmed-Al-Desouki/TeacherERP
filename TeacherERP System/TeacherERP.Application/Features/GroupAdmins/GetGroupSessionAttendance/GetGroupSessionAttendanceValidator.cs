// جديد — TeacherERP.Application/Features/GroupAdmins/GetGroupSessionAttendance/GetGroupSessionAttendanceValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.GroupAdmins.GetGroupSessionAttendance
{
    public class GetGroupSessionAttendanceValidator : AbstractValidator<GetGroupSessionAttendanceQuery>
    {
        public GetGroupSessionAttendanceValidator()
        {
            Include(new PagedRequestRules<GetGroupSessionAttendanceQuery>());
        }
    }
}