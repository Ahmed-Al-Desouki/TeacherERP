// جديد — TeacherERP.Application/Features/Attendance/AdminScan/AdminScanValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Attendance.AdminScan
{
    public class AdminScanValidator : AbstractValidator<AdminScanCommand>
    {
        public AdminScanValidator()
        {
            RuleFor(x => x.StudentQrCode)
                .NotEmpty().WithMessage("Student card code is required.")
                .MaximumLength(64).WithMessage("Student card code is invalid.");
        }
    }
}