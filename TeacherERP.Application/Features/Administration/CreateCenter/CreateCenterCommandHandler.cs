using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration.CreateCenter
{
    public class CreateCenterCommandHandler : IRequestHandler<CreateCenterCommand, Result<AdminCenterDto>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCenterCommandHandler(
            ITenantRepository tenantRepository,
            ICenterRepository centerRepository,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AdminCenterDto>> Handle(CreateCenterCommand request, CancellationToken cancellationToken)
        {
            var ownerTenant = await _tenantRepository.GetByOwnerIdAsync(request.OwnerUserId, cancellationToken);
            if (ownerTenant is null)
            {
                return Result<AdminCenterDto>.Failure(AdministrationErrors.TeacherNotFound);
            }

            var center = Center.Create(request.Name, request.OwnerUserId);
            _centerRepository.Add(center);
            _centerRepository.AddCenterTeacher(CenterTeacher.CreateFounder(center.Id, ownerTenant.Id));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AdminCenterDto>.Success(new AdminCenterDto(center.Id, center.Name, center.OwnerUserId));
        }
    }
}
