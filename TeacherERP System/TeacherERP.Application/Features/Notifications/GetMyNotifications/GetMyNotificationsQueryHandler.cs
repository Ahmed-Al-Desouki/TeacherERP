// موجود (تعديل) — TeacherERP.Application/Features/Notifications/GetMyNotifications/GetMyNotificationsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Notifications.GetMyNotifications
{
    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<PagedResult<NotificationItem>>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationRepository _repository;

        public GetMyNotificationsQueryHandler(ICurrentUserService currentUser, INotificationRepository repository)
        {
            _currentUser = currentUser;
            _repository = repository;
        }

        public async Task<Result<PagedResult<NotificationItem>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<PagedResult<NotificationItem>>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var page = await _repository.GetMinePageAsync(userId, request.UnreadOnly, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<NotificationItem>>.Success(page);
        }
    }
}
