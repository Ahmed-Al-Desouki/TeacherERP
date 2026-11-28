// جديد — TeacherERP.Application/Features/TeacherRequests/Status/GetTeacherRequestStatusQueryHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Status
{
    public class GetTeacherRequestStatusQueryHandler : IRequestHandler<GetTeacherRequestStatusQuery, Result<TeacherRequestStatusDto>>
    {
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly TimeProvider _timeProvider;

        public GetTeacherRequestStatusQueryHandler(ITeacherAccessRequestRepository requestRepository, TimeProvider timeProvider)
        {
            _requestRepository = requestRepository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<TeacherRequestStatusDto>> Handle(GetTeacherRequestStatusQuery request, CancellationToken cancellationToken)
        {
            var found = await _requestRepository.GetByTokenHashAsync(TeacherRequestToken.Hash(request.Token), cancellationToken);
            if (found is null)
            {
                return Result<TeacherRequestStatusDto>.Failure(TeacherRequestErrors.NotFound);
            }

            var status = found.GetEffectiveStatus(_timeProvider.GetUtcNow().UtcDateTime);

            // سبب الرفض فقط يظهر لصاحب الطلب
            var note = status == TeacherAccessRequestStatus.Rejected ? found.DecisionNote : null;

            return Result<TeacherRequestStatusDto>.Success(new TeacherRequestStatusDto(status, found.ExpiresAtUtc, note));
        }
    }
}
