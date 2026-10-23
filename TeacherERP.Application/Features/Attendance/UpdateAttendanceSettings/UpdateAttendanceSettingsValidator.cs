// جديد — TeacherERP.Application/Features/Attendance/UpdateAttendanceSettings/UpdateAttendanceSettingsValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings
{
    public class UpdateAttendanceSettingsValidator : AbstractValidator<UpdateAttendanceSettingsCommand>
    {
        public UpdateAttendanceSettingsValidator()
        {
            RuleFor(x => x.Mode)
                .IsInEnum().WithMessage("Attendance mode is invalid.");
        }
    }
}