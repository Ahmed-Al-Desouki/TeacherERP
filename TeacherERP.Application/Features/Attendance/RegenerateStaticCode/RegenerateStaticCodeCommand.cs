// جديد — TeacherERP.Application/Features/Attendance/RegenerateStaticCode/RegenerateStaticCodeCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.RegenerateStaticCode
{
    // تغيير سر الحضور (يبطل الكود الثابت المطبوع والمتغير الحالي) لو تسرّبت صورة الكود (للمدرس فقط)
    public record RegenerateStaticCodeCommand : IRequest<Result<AttendanceSettingsDto>>;
}