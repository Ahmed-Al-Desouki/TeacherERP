// جديد — TeacherERP.Application/Features/Notifications/MarkAllNotificationsRead/MarkAllNotificationsReadCommandHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.MarkAllNotificationsRead
{
    public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAllNotificationsReadCommandHandler(
            ICurrentUserService currentUser,
            INotificationRepository repository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var unread = await _repository.GetOwnUnreadTrackedAsync(userId, cancellationToken);
            foreach (var notification in unread)
            {
                notification.MarkRead();
            }

            if (unread.Count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Result<bool>.Success(true);
        }
    }
}