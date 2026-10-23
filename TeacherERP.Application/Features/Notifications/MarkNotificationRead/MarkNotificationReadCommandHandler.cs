// جديد — TeacherERP.Application/Features/Notifications/MarkNotificationRead/MarkNotificationReadCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.MarkNotificationRead
{
    public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkNotificationReadCommandHandler(
            ICurrentUserService currentUser,
            INotificationRepository repository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            // إشعاري أنا فقط (غير الموجود وإشعار غيري نفس الرد)
            var notification = await _repository.GetOwnTrackedAsync(request.NotificationId, userId, cancellationToken);
            if (notification is null)
            {
                return Result<bool>.Failure(new Error("Notification.NotFound", "The notification was not found."));
            }

            notification.MarkRead();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}