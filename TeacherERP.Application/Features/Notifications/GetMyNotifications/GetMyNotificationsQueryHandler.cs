// جديد — TeacherERP.Application/Features/Notifications/GetMyNotifications/GetMyNotificationsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.GetMyNotifications
{
    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<List<NotificationItem>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationRepository _repository;

        public GetMyNotificationsQueryHandler(ICurrentUserService currentUser, INotificationRepository repository)
        {
            _currentUser = currentUser;
            _repository = repository;
        }

        public async Task<Result<List<NotificationItem>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<List<NotificationItem>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var items = await _repository.GetMineAsync(userId, request.UnreadOnly, cancellationToken);
            return Result<List<NotificationItem>>.Success(items);
        }
    }
}