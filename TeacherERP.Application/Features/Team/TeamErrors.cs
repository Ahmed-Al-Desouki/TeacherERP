// جديد — TeacherERP.Application/Features/Team/TeamErrors.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team
{
    public static class TeamErrors
    {
        // نفس الخطأ لإيميل طالب أو مدرس أو حساب موقوف، حتى لا نكشف نوع حساب أي إيميل
        public static readonly Error EmailNotAvailable = new("Team.EmailNotAvailable", "This email cannot be used as an assistant.");
        public static readonly Error AlreadyMember = new("Team.AlreadyMember", "This person is already on your team.");
        public static readonly Error LimitReached = new("Team.LimitReached", "You have reached the maximum number of assistants.");
        public static readonly Error EmailSendFailed = new("Team.EmailSendFailed", "The welcome email could not be sent, so the account was not created. Please try again.");
    }
}