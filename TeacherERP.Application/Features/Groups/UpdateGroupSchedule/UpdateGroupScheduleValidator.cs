// جديد — TeacherERP.Application/Features/Groups/UpdateGroupSchedule/UpdateGroupScheduleValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.Groups.UpdateGroupSchedule
{
    public class UpdateGroupScheduleValidator : AbstractValidator<UpdateGroupScheduleCommand>
    {
        public UpdateGroupScheduleValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Group id is required.");

            Include(new GroupSlotsRules<UpdateGroupScheduleCommand>());
        }
    }
}