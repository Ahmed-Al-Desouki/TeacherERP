// جديد — TeacherERP.Application/Features/SessionContent/UploadSessionAttachment/UploadSessionAttachmentCommandHandler.cs
using Hangfire;
using MediatR;
using TeacherERP.Application.Features.GroupAdmins;
using TeacherERP.Application.Features.Schedule;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.SessionContent;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.SessionContent.UploadSessionAttachment
{
    public class UploadSessionAttachmentCommandHandler
        : IRequestHandler<UploadSessionAttachmentCommand, Result<SessionAttachmentDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IGroupSessionRepository _sessionRepository;
        private readonly IGroupAccessService _groupAccess;
        private readonly ISessionAttachmentRepository _attachmentRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IAuditRepository _auditRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly TimeProvider _timeProvider;

        public UploadSessionAttachmentCommandHandler(
            ICurrentUserService currentUser,
            ICurrentTenantAccessor currentTenant,
            IGroupSessionRepository sessionRepository,
            IGroupAccessService groupAccess,
            ISessionAttachmentRepository attachmentRepository,
            IFileStorage fileStorage,
            IAuditRepository auditRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IBackgroundJobClient backgroundJobClient,
            TimeProvider timeProvider)
        {
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _sessionRepository = sessionRepository;
            _groupAccess = groupAccess;
            _attachmentRepository = attachmentRepository;
            _fileStorage = fileStorage;
            _auditRepository = auditRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _backgroundJobClient = backgroundJobClient;
            _timeProvider = timeProvider;
        }

        public async Task<Result<SessionAttachmentDto>> Handle(
            UploadSessionAttachmentCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<SessionAttachmentDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            if (_currentTenant.Current is not { } tenant)
            {
                return Result<SessionAttachmentDto>.Failure(TenantErrors.Required);
            }

            // الفلتر العام يضمن أن الحصة تخص المدرس الحالي فقط
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
            if (session is null)
            {
                return Result<SessionAttachmentDto>.Failure(ScheduleErrors.NotFound);
            }

            if (session.IsCancelled)
            {
                return Result<SessionAttachmentDto>.Failure(ScheduleErrors.Cancelled);
            }

            var access = await _groupAccess.EnsureCanManageGroupAsync(session.GroupId, cancellationToken);
            if (access.IsFailure)
            {
                return Result<SessionAttachmentDto>.Failure(access.Error);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            if (access.Value == GroupAccessLevel.GroupAdmin
                && !SessionContentPolicy.IsInAdminWindow(session.StartTime, session.EndTime, now))
            {
                return Result<SessionAttachmentDto>.Failure(SessionContentErrors.OutsideAdminWindow);
            }

            // نحتاج Stream نقدر نرجّعه للبداية بعد قراءة الـ Header (الطلب محدود بـ 11MB فلا خطر على الذاكرة)
            var stream = request.Content;
            MemoryStream? ownedCopy = null;
            if (!stream.CanSeek)
            {
                ownedCopy = new MemoryStream();
                await stream.CopyToAsync(ownedCopy, cancellationToken);
                ownedCopy.Position = 0;
                stream = ownedCopy;
            }

            try
            {
                if (stream.Length == 0)
                {
                    return Result<SessionAttachmentDto>.Failure(SessionContentErrors.EmptyFile);
                }

                if (stream.Length > SessionContentPolicy.MaxFileBytes)
                {
                    return Result<SessionAttachmentDto>.Failure(SessionContentErrors.FileTooLarge);
                }

                if (await _attachmentRepository.CountAsync(session.Id, cancellationToken) >= SessionContentPolicy.MaxFilesPerSession)
                {
                    return Result<SessionAttachmentDto>.Failure(SessionContentErrors.LimitReached);
                }

                // النوع الحقيقي من أول البايتات
                var header = new byte[FileSignature.HeaderLength];
                var read = 0;
                while (read < header.Length)
                {
                    var n = await stream.ReadAsync(header.AsMemory(read), cancellationToken);
                    if (n == 0)
                    {
                        break;
                    }

                    read += n;
                }

                stream.Position = 0;

                var type = FileSignature.Detect(header.AsSpan(0, read));
                if (type == DetectedFileType.Unknown)
                {
                    return Result<SessionAttachmentDto>.Failure(SessionContentErrors.UnsupportedType);
                }

                var fileName = FileNames.Sanitize(request.FileName, type);
                var folder = $"teacher-erp/{tenant.TenantId:N}/sessions/{session.Id:N}";

                var uploaded = await _fileStorage.UploadAsync(stream, fileName, folder, cancellationToken);
                if (uploaded.IsFailure)
                {
                    return Result<SessionAttachmentDto>.Failure(uploaded.Error);
                }

                var publicId = uploaded.Value!.PublicId;

                var attachment = SessionAttachment.Create(
                    tenant.TenantId, session.Id, request.Kind, uploaded.Value.Url, publicId, fileName,
                    FileSignature.ContentTypeOf(type), stream.Length, request.Caption, userId);

                try
                {
                    _attachmentRepository.Add(attachment);
                    _auditRepository.Add(AuditLog.Create(
                        tenant.TenantId, userId, AuditAction.SessionAttachmentUploaded, null, $"{request.Kind}: {fileName}"));

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch
                {
                    // الملف اترفع والحفظ فشل: نمسحه حتى لا يبقى يتيماً
                    _backgroundJobClient.Enqueue<IStoredFileCleanupJob>(job => job.DeleteAsync(publicId));
                    throw;
                }

                var uploader = await _userRepository.GetByIdAsync(userId, cancellationToken);

                return Result<SessionAttachmentDto>.Success(SessionAttachmentDto.From(attachment, uploader?.FullName));
            }
            finally
            {
                if (ownedCopy is not null)
                {
                    await ownedCopy.DisposeAsync();
                }
            }
        }
    }
}