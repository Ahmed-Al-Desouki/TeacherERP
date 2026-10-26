// جديد — TeacherERP.Application/Features/Users/Commands/Auth/ChangePassword/ChangePasswordValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            // نفس سياسة Identity (6 أحرف على الأقل + رقم) حتى لا يُرفض الباسورد بعد التحقق
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .NotEqual(x => x.CurrentPassword).WithMessage("The new password must be different from the current password.");
        }
    }
}