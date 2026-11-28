using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration.LinkTeacherToCenter
{
    public class LinkTeacherToCenterCommandHandler : IRequestHandler<LinkTeacherToCenterCommand, Result<CenterTeacherLinkDto>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICenterRepository _centerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LinkTeacherToCenterCommandHandler(
            ITenantRepository tenantRepository,
            ICenterRepository centerRepository,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _centerRepository = centerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CenterTeacherLinkDto>> Handle(LinkTeacherToCenterCommand request, CancellationToken cancellationToken)
        {
            var center = await _centerRepository.GetByIdAsync(request.CenterId, cancellationToken);
            if (center is null)
            {
                return Result<CenterTeacherLinkDto>.Failure(AdministrationErrors.CenterNotFound);
            }

            var teacherTenant = await _tenantRepository.GetByOwnerIdAsync(request.TeacherUserId, cancellationToken);
            if (teacherTenant is null)
            {
                return Result<CenterTeacherLinkDto>.Failure(AdministrationErrors.TeacherNotFound);
            }

            var link = await _centerRepository.GetCenterTeacherAsync(center.Id, teacherTenant.Id, cancellationToken);
            if (link is null)
            {
                link = CenterTeacher.CreateActive(center.Id, teacherTenant.Id, CenterTeacherOrigin.Administrator);
                _centerRepository.AddCenterTeacher(link);
            }
            else if (link.Status == CenterTeacherStatus.Active)
            {
                return Result<CenterTeacherLinkDto>.Failure(AdministrationErrors.AlreadyLinked);
            }
            else
            {
                // كان معلّقاً أو أُزيل سابقاً: يعود نشطاً بقرار مدير النظام
                link.Activate();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CenterTeacherLinkDto>.Success(
                new CenterTeacherLinkDto(center.Id, teacherTenant.Id, request.TeacherUserId, link.Status));
        }
    }
}
