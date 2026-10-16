using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers.UpdateCenter
{
    public class UpdateCenterCommandHandler : IRequestHandler<UpdateCenterCommand, Result<CenterDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCenterCommandHandler(
            ICurrentUserService currentUser,
            ICenterRepository centerRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CenterDto>> Handle(UpdateCenterCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<CenterDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            // غير الموجود وغير المملوك لي نفس الرد (لا نكشف وجود سناتر)
            var center = await _centerRepository.GetByIdAsync(request.CenterId, cancellationToken);
            if (center is null || center.OwnerUserId != userId)
            {
                return Result<CenterDto>.Failure(CenterErrors.NotFound);
            }

            center.UpdateDetails(request.Name, request.Address, request.Latitude, request.Longitude, request.GeofenceRadiusMeters);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CenterDto>.Success(CenterDto.From(center, userId));
        }
    }
}