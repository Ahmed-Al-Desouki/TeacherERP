using FluentValidation;

namespace TeacherERP.Application.Features.Centers.UpdateCenter
{
    public class UpdateCenterValidator : AbstractValidator<UpdateCenterCommand>
    {
        public UpdateCenterValidator()
        {
            RuleFor(x => x.CenterId)
                .NotEmpty().WithMessage("Center id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Center name is required.")
                .MaximumLength(150).WithMessage("Center name must not exceed 150 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(300).WithMessage("Address must not exceed 300 characters.");

            // الإحداثيات اختيارية، لكن تُرسل معاً أو لا تُرسل
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90d, 90d).WithMessage("Latitude must be between -90 and 90.")
                .When(x => x.Latitude.HasValue);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180d, 180d).WithMessage("Longitude must be between -180 and 180.")
                .When(x => x.Longitude.HasValue);

            RuleFor(x => x.Latitude)
                .NotNull().WithMessage("Latitude and longitude must be sent together.")
                .When(x => x.Longitude.HasValue);

            RuleFor(x => x.Longitude)
                .NotNull().WithMessage("Latitude and longitude must be sent together.")
                .When(x => x.Latitude.HasValue);

            RuleFor(x => x.GeofenceRadiusMeters)
                .InclusiveBetween(10, 500).WithMessage("Geofence radius must be between 10 and 500 meters.");
        }
    }
}