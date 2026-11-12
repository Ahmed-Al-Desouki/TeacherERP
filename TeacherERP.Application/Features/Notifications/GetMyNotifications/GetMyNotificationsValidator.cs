// جديد — TeacherERP.Application/Features/Notifications/GetMyNotifications/GetMyNotificationsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Notifications.GetMyNotifications
{
    public class GetMyNotificationsValidator : AbstractValidator<GetMyNotificationsQuery>
    {
        public GetMyNotificationsValidator()
        {
            Include(new PagedRequestRules<GetMyNotificationsQuery>());
        }
    }
}
