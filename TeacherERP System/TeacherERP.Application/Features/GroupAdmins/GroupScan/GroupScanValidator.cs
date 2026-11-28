// جديد — TeacherERP.Application/Features/GroupAdmins/GroupScan/GroupScanValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.GroupAdmins.GroupScan
{
    public class GroupScanValidator : AbstractValidator<GroupScanCommand>
    {
        public GroupScanValidator()
        {
            RuleFor(x => x.StudentQrCode)
                .NotEmpty().WithMessage("Student card code is required.")
                .MaximumLength(64).WithMessage("Student card code is invalid.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90d, 90d).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180d, 180d).WithMessage("Longitude must be between -180 and 180.");
        }
    }
}