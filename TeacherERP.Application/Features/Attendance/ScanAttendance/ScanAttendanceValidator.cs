// جديد — TeacherERP.Application/Features/Attendance/ScanAttendance/ScanAttendanceValidator.cs
using FluentValidation;
using TeacherERP.Application.Attendance;

namespace TeacherERP.Application.Features.Attendance.ScanAttendance
{
    public class ScanAttendanceValidator : AbstractValidator<ScanAttendanceCommand>
    {
        public ScanAttendanceValidator()
        {
            RuleFor(x => x.TeacherId)
                .NotEmpty().WithMessage("Teacher id is required.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(64).WithMessage("Code is invalid.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90d, 90d).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180d, 180d).WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.AccuracyMeters)
                .NotNull().WithMessage("Location accuracy is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Accuracy must not be negative.")
                .LessThanOrEqualTo(AttendancePolicy.MaximumLocationAccuracyMeters)
                .WithMessage($"Accuracy must not exceed {AttendancePolicy.MaximumLocationAccuracyMeters} meters.");
        }
    }
}