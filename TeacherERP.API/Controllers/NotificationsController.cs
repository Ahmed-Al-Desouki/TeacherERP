// جديد — TeacherERP.API/Controllers/NotificationsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Notifications.GetMyNotifications;
using TeacherERP.Application.Features.Notifications.MarkAllNotificationsRead;
using TeacherERP.Application.Features.Notifications.MarkNotificationRead;

namespace TeacherERP.API.Controllers
{
    // إشعاراتي (عبر كل مدرسيّ، بدون تحديد مدرس)
    [Authorize]
    public class NotificationsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ?unreadOnly=true
        [HttpGet]
        public async Task<IActionResult> GetMine([FromQuery] bool unreadOnly, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyNotificationsQuery(unreadOnly), cancellationToken);
            return ToResponse(result);
        }

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new MarkNotificationReadCommand(id), cancellationToken);
            return ToResponse(result, new { Message = "Marked as read." });
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new MarkAllNotificationsReadCommand(), cancellationToken);
            return ToResponse(result, new { Message = "All notifications were marked as read." });
        }
    }
}