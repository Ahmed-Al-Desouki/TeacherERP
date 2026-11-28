using FluentValidation;

namespace TeacherERP.Application.Features.Groups.JoinCodes;

public sealed class UpdateGroupJoinCodesValidator : AbstractValidator<UpdateGroupJoinCodesCommand>
{
    public UpdateGroupJoinCodesValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
    }
}
