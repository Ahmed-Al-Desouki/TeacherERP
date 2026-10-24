// موجود (تعديل) — TeacherERP.Application/Attendance/NotificationService.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Attendance
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task NotifyStaffAsync(
            Guid tenantId, NotificationType type, string message, Guid? relatedId, CancellationToken cancellationToken)
        {
            var recipients = await _repository.GetStaffRecipientIdsAsync(tenantId, cancellationToken);

            foreach (var recipientId in recipients)
            {
                _repository.Add(Notification.Create(recipientId, tenantId, type, message, relatedId));
            }
        }
    }
}