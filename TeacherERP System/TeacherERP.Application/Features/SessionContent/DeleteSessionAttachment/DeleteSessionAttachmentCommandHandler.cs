// جديد — TeacherERP.Application/Features/SessionContent/DeleteSessionAttachment/DeleteSessionAttachmentCommandHandler.cs
using Hangfire;
using MediatR;
using TeacherERP.Application.Features.Schedule;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.SessionContent;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment
{
    public class DeleteSessionAttachmentCommandHandler : IRequestHandler<DeleteSessionAttachmentCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IGroupAccessService _groupAccess;
        private readonly ISessionAttachmentRepository _attachmentRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly TimeProvider _timeProvider;

        public DeleteSessionAttachmentCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            IGroupAccessService groupAccess,
            ISessionAttachmentRepository attachmentRepository,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork,
            IBackgroundJobClient backgroundJobClient,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _groupAccess = groupAccess;
            _attachmentRepository = attachmentRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _backgroundJobClient = backgroundJobClient;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(DeleteSessionAttachmentCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن الملف يخص المدرس الحالي فقط
            var attachment = await _attachmentRepository.GetByIdAsync(request.AttachmentId, cancellationToken);
            if (attachment is null)
            {
                return Result<bool>.Failure(SessionContentErrors.AttachmentNotFound);
            }

            var session = await _sessionRepository.GetByIdAsync(attachment.SessionId, cancellationToken);
            if (session is null)
            {
                return Result<bool>.Failure(SessionContentErrors.AttachmentNotFound);
            }

            var access = await _groupAccess.EnsureCanManageGroupAsync(session.GroupId, cancellationToken);
            if (access.IsFailure)
            {
                return Result<bool>.Failure(access.Error);
            }

            if (access.Value == GroupAccessLevel.GroupAdmin)
            {
                if (attachment.UploadedByUserId != userId)
                {
                    return Result<bool>.Failure(SessionContentErrors.OnlyOwnFiles);
                }

                var now = _timeProvider.GetUtcNow().UtcDateTime;
                if (!SessionContentPolicy.IsInAdminWindow(session.StartTime, session.EndTime, now))
                {
                    return Result<bool>.Failure(SessionContentErrors.OutsideAdminWindow);
                }
            }

            var publicId = attachment.PublicId;

            _attachmentRepository.Remove(attachment);
            _auditRepository.Add(AuditLog.Create(
                tenant.TenantId, userId, AuditAction.SessionAttachmentDeleted, null, $"{attachment.Kind}: {attachment.FileName}"));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // السجل اتمسح فالمستخدم لا ينتظر Cloudinary. الـ Job تعيد المحاولة تلقائياً لو فشلت
            _backgroundJobClient.Enqueue<IStoredFileCleanupJob>(job => job.DeleteAsync(publicId));

            return Result<bool>.Success(true);
        }
    }
}