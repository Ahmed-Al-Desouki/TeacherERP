using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.ResetToken)
                .NotEmpty().WithMessage("Reset token is required.")
                .MaximumLength(128).WithMessage("Reset token is invalid.");

            // نفس سياسة Identity (6 أحرف على الأقل + رقم) حتى لا يُرفض الباسورد بعد التحقق
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        }
    }
}