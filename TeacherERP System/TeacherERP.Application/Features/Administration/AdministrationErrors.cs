using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration
{
    public static class AdministrationErrors
    {
        // مدير النظام وحده يستدعي هذه الأوامر، فلا داعي لإخفاء وجود الإيميل
        public static readonly Error EmailInUse = new("Administration.EmailInUse", "This email is already used by another account.");
        public static readonly Error EmailSendFailed = new("Administration.EmailSendFailed", "The welcome email could not be sent, so the teacher account was not created. Please try again.");
        public static readonly Error TeacherNotFound = new("Administration.TeacherNotFound", "The teacher was not found.");
        public static readonly Error CenterNotFound = new("Administration.CenterNotFound", "The center was not found.");
        public static readonly Error AlreadyLinked = new("Administration.AlreadyLinked", "This teacher is already active in this center.");
    }
}
