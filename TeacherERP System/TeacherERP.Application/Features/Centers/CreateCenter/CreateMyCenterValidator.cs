using FluentValidation;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Centers.CreateCenter
{
    public class CreateMyCenterValidator : AbstractValidator<CreateMyCenterCommand>
    {
        public CreateMyCenterValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(150);
            RuleFor(x => x.Address).MaximumLength(300);
            RuleFor(x => x.Latitude).NotNull().InclusiveBetween(-90d, 90d);
            RuleFor(x => x.Longitude).NotNull().InclusiveBetween(-180d, 180d);
            RuleFor(x => x.GeofenceRadiusMeters).InclusiveBetween(10, 500);
        }
    }
}
