// جديد — TeacherERP.Application/Features/SessionContent/GetSessionAttachments/GetSessionAttachmentsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Schedule;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.SessionContent.GetSessionAttachments
{
    public class GetSessionAttachmentsQueryHandler
        : IRequestHandler<GetSessionAttachmentsQuery, Result<PagedResult<SessionAttachmentDto>>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IGroupAccessService _groupAccess;
        private readonly ISessionAttachmentRepository _attachmentRepository;

        public GetSessionAttachmentsQueryHandler(
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            IGroupAccessService groupAccess,
            ISessionAttachmentRepository attachmentRepository)
        {
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _groupAccess = groupAccess;
            _attachmentRepository = attachmentRepository;
        }

        public async Task<Result<PagedResult<SessionAttachmentDto>>> Handle(
            GetSessionAttachmentsQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<SessionAttachmentDto>>.Failure(TenantErrors.Required);
            }

            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session is null)
            {
                return Result<PagedResult<SessionAttachmentDto>>.Failure(ScheduleErrors.NotFound);
            }

            var access = await _groupAccess.EnsureCanManageGroupAsync(session.GroupId, cancellationToken);
            if (access.IsFailure)
            {
                return Result<PagedResult<SessionAttachmentDto>>.Failure(access.Error);
            }

            var page = await _attachmentRepository.GetPageAsync(
                session.Id, request.Kind, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<SessionAttachmentDto>>.Success(page.Map(SessionAttachmentDto.From));
        }
    }
}