using FluentValidation;

namespace TeacherERP.Application.Features.Users.Commands.Auth.SetPassword;

public sealed class SetPasswordValidator : AbstractValidator<SetPasswordCommand>
{
    public SetPasswordValidator()
    {
        RuleFor(command => command.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
    }
}
