using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.CreateCenter
{
    public class CreateMyCenterCommandHandler : IRequestHandler<CreateMyCenterCommand, Result<CenterDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICenterProvisioningService _centerProvisioning;

        public CreateMyCenterCommandHandler(ICurrentUserService currentUser, ICenterProvisioningService centerProvisioning)
        {
            _currentUser = currentUser;
            _centerProvisioning = centerProvisioning;
        }

        public async Task<Result<CenterDto>> Handle(CreateMyCenterCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<CenterDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var created = await _centerProvisioning.CreateAsync(
                userId, request.Name, request.Address, request.Latitude, request.Longitude,
                request.GeofenceRadiusMeters, cancellationToken);

            return created.IsFailure
                ? Result<CenterDto>.Failure(created.Error)
                : Result<CenterDto>.Success(CenterDto.From(created.Value!, userId));
        }
    }
}
