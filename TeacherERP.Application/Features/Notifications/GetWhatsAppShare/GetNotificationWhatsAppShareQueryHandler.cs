using MediatR;
using TeacherERP.Application.Features.Notifications;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.GetWhatsAppShare;

public sealed class GetNotificationWhatsAppShareQueryHandler
    : IRequestHandler<GetNotificationWhatsAppShareQuery, Result<NotificationWhatsAppShareDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationRepository _repository;

    public GetNotificationWhatsAppShareQueryHandler(
        ICurrentUserService currentUser,
        INotificationRepository repository)
    {
        _currentUser = currentUser;
        _repository = repository;
    }

    public async Task<Result<NotificationWhatsAppShareDto>> Handle(
        GetNotificationWhatsAppShareQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            return Result<NotificationWhatsAppShareDto>.Failure(
                new Error("Auth.Unauthorized", "You must be logged in."));
        }

        var share = await _repository.GetOwnSessionAttachmentShareAsync(
            request.NotificationId, userId, cancellationToken);

        if (share is null)
        {
            return Result<NotificationWhatsAppShareDto>.Failure(NotificationErrors.ShareUnavailable);
        }

        var kind = share.Kind switch
        {
            SessionAttachmentKind.Homework => "واجب جديد",
            SessionAttachmentKind.HomeworkAnswer => "إجابة نموذجية جديدة",
            SessionAttachmentKind.Explanation => "شرح جديد",
            SessionAttachmentKind.Board => "صورة سبورة جديدة",
            _ => "ملف جديد للحصة"
        };
        var title = string.IsNullOrWhiteSpace(share.Caption)
            ? share.FileName
            : $"{share.Caption.Trim()} ({share.FileName})";
        var shareText = $"📚 {kind} لمجموعة {share.GroupName}\n{title}\n{share.AttachmentUrl}";
        var composeUrl = $"https://wa.me/?text={Uri.EscapeDataString(shareText)}";

        return Result<NotificationWhatsAppShareDto>.Success(
            new NotificationWhatsAppShareDto(shareText, composeUrl, share.WhatsAppGroupLink));
    }
}
