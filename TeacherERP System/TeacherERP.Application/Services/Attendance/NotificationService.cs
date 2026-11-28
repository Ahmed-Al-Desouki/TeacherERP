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

        public Task NotifyUserAsync(
            Guid tenantId, Guid recipientUserId, NotificationType type,
            string message, Guid? relatedId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Add(recipientUserId, tenantId, type, message, relatedId);
            return Task.CompletedTask;
        }

        public async Task NotifyTeacherAsync(
            Guid tenantId, NotificationType type, string message, Guid? relatedId, CancellationToken cancellationToken)
        {
            var teacherId = await _repository.GetTeacherRecipientIdAsync(tenantId, cancellationToken);
            Add(teacherId, tenantId, type, message, relatedId);
        }

        public async Task NotifyStaffAsync(
            Guid tenantId, NotificationType type, string message, Guid? relatedId, CancellationToken cancellationToken)
        {
            var recipients = await _repository.GetStaffRecipientIdsAsync(tenantId, cancellationToken);

            foreach (var recipientId in recipients)
            {
                Add(recipientId, tenantId, type, message, relatedId);
            }
        }

        public async Task NotifyGroupStudentsAsync(
            Guid tenantId, Guid groupId, NotificationType type,
            string message, Guid relatedId, CancellationToken cancellationToken)
        {
            var recipients = await _repository.GetGroupStudentRecipientIdsAsync(
                tenantId, groupId, cancellationToken);

            foreach (var recipientId in recipients)
            {
                Add(recipientId, tenantId, type, message, relatedId);
            }
        }

        private void Add(Guid recipientUserId, Guid tenantId, NotificationType type, string message, Guid? relatedId)
        {
            var boundedMessage = message.Length <= 300 ? message : message[..300];
            _repository.Add(Notification.Create(recipientUserId, tenantId, type, boundedMessage, relatedId));
        }
    }
}
