// جديد — TeacherERP.Application/Features/Attendance/ManualAttendance/ManualAttendanceValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Attendance.ManualAttendance
{
    public class ManualAttendanceValidator : AbstractValidator<ManualAttendanceCommand>
    {
        public ManualAttendanceValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student id is required.");

            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session id is required.");

            // أقل من حد الـ Audit (500) لأن رقم الحصة بيتضاف للسبب
            RuleFor(x => x.Reason)
                .MaximumLength(400).WithMessage("Reason must not exceed 400 characters.");
        }
    }
}