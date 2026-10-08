using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    // مسؤول عن التحقق من صحة المدخلات فقط (بدون أي Business Logic)
    public class CreateStudentValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            // نفس سياسة Identity (6 أحرف على الأقل + رقم) حتى لا يُرفض الباسورد بعد التحقق من الـ OTP
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        }
    }
}